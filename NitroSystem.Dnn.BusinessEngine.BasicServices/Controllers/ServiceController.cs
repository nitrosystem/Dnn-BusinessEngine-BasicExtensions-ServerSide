using DotNetNuke.Web.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NitroSystem.Dnn.BusinessEngine.Api.ViewModels;
using NitroSystem.Dnn.BusinessEngine.BasicServices.Enums;
using NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels.Database.CustomQuery;
using NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels.Database.DataSource;
using NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels.Database.SubmitEntity;
using NitroSystem.Dnn.BusinessEngine.Core;
using NitroSystem.Dnn.BusinessEngine.Data.Entities.Tables;
using NitroSystem.Dnn.BusinessEngine.Data.Repositories;
using NitroSystem.Dnn.BusinessEngine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using NitroSystem.Dnn.BusinessEngine.BasicServices.DB.Entities;
using NitroSystem.Dnn.BusinessEngine.BasicServices.DB.Repositories;
using NitroSystem.Dnn.BusinessEngine.Api.Mapping;
using System.IO;
using DotNetNuke.Entities.Host;
using NitroSystem.Dnn.BusinessEngine.BasicServices.PublicServices;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.Controllers
{
    public class ServiceController : DnnApiController
    {
        #region Studio Api

        #region Custom Query Service

        [DnnAuthorize(StaticRoles = "Administrators")]
        [HttpGet]
        public HttpResponseMessage GetCustomQuery(Guid serviceID)
        {
            try
            {
                var customQueryService = CustomQueryServiceRepository.GetCustomQueryService(serviceID);

                if (customQueryService != null && customQueryService.DatabaseObjectType == DatabaseObjectType.StoredProcedure)
                {
                    customQueryService.Query = General.GetSpScript(customQueryService.StoredProcedureName);
                    customQueryService.Query = customQueryService.Query.Replace("dbo."+ customQueryService.StoredProcedureName, "{Schema}.{ProcedureName}");
                }

                return Request.CreateResponse(HttpStatusCode.OK, customQueryService);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = ex.Message });
            }
        }

        [DnnAuthorize(StaticRoles = "Administrators")]
        [HttpPost]
        public HttpResponseMessage CreateCustomQuery([FromBody] CustomQueryServiceViewModel customQueryService, [FromUri] bool isNewService = false)
        {
            try
            {
                var service = customQueryService.Service;

                customQueryService.ServiceID = service.ServiceID;

                if (customQueryService.DatabaseObjectType == DatabaseObjectType.StoredProcedure)
                {
                    string dropQuery = string.Format("IF OBJECT_ID('{0}.{1}', 'P') IS NOT NULL \n\t DROP PROCEDURE {0}.{1}; \nGO\n", "dbo", customQueryService.StoredProcedureName);

                    string customQuery = dropQuery + customQueryService.Query;
                    customQuery = customQuery.Replace("{Schema}", "dbo");
                    customQuery = customQuery.Replace("{ProcedureName}", customQueryService.StoredProcedureName);

                    DbUtil.ExecuteTransaction(customQuery);

                    customQueryService.Query = null;

                    service.Params = General.GetSpParams(customQueryService.StoredProcedureName);
                }

                var objCustomQueryServiceInfo = new CustomQueryServiceInfo()
                {
                    ItemID = customQueryService.ItemID,
                    ServiceID = customQueryService.ServiceID,
                    DatabaseObjectType = customQueryService.DatabaseObjectType,
                    StoredProcedureName = customQueryService.StoredProcedureName,
                    Query = customQueryService.Query,
                    Settings = JsonConvert.SerializeObject(customQueryService.Settings)
                };

                if (customQueryService.ItemID == Guid.Empty)
                    customQueryService.ItemID = CustomQueryServiceRepository.AddCustomQueryService(objCustomQueryServiceInfo);
                else
                    CustomQueryServiceRepository.UpdateCustomQueryService(objCustomQueryServiceInfo);

                General.SaveServiceParams(customQueryService.ServiceID, service.Params);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    ItemID = customQueryService.ItemID,
                    ServiceParams = service.Params
                });
            }
            catch (Exception ex)
            {
                if (isNewService) ServiceRepository.Instance.DeleteService(customQueryService.Service.ServiceID);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = ex.Message, IsServiceDeleted = isNewService });
            }
        }

        #endregion

        #region Submit Entity Service

        [DnnAuthorize(StaticRoles = "Administrators")]
        [HttpGet]
        public HttpResponseMessage GetSubmitEntityService()
        {
            return GetSubmitEntityService(Guid.Empty);
        }

        [DnnAuthorize(StaticRoles = "Administrators")]
        [HttpGet]
        public HttpResponseMessage GetSubmitEntityService(Guid serviceID)
        {
            try
            {
                var submitEntityService = SubmitEntityServiceMapping.GetSubmitEntityServiceViewModel(serviceID);

                submitEntityService = submitEntityService ?? new SubmitEntityServiceViewModel()
                {
                    QueryType = QueryType.QueryDesigner,
                    DatabaseObjectType = DatabaseObjectType.StoredProcedure,
                    ActionType = ActionType.InsertAndUpdate
                };

                if (string.IsNullOrEmpty(submitEntityService.BaseQuery))
                {
                    submitEntityService.BaseQuery = FileUtil.GetFileContent(HttpContext.Current.Server.MapPath("~/DesktopModules/BusinessEngine/extensions/basic/services/sql-templates/submit-entity/insert&update-entity.sql")); ;
                    submitEntityService.InsertBaseQuery = FileUtil.GetFileContent(HttpContext.Current.Server.MapPath("~/DesktopModules/BusinessEngine/extensions/basic/services/sql-templates/submit-entity/insert-entity.sql")); ;
                    submitEntityService.UpdateBaseQuery = FileUtil.GetFileContent(HttpContext.Current.Server.MapPath("~/DesktopModules/BusinessEngine/extensions/basic/services/sql-templates/submit-entity/update-entity.sql")); ;
                }

                if (submitEntityService.QueryType == QueryType.CustomQuery && !string.IsNullOrEmpty(submitEntityService.StoredProcedureName))
                {
                    submitEntityService.CustomQuery = General.GetSpScript(submitEntityService.StoredProcedureName);

                    submitEntityService.Service = new ServiceViewModel();
                    submitEntityService.Service.Params = General.GetSpParams(submitEntityService.StoredProcedureName);
                }

                return Request.CreateResponse(HttpStatusCode.OK, submitEntityService);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [DnnAuthorize(StaticRoles = "Administrators")]
        [HttpPost]
        public HttpResponseMessage CreateSubmitEntityService([FromBody] SubmitEntityServiceViewModel submitEntityService, [FromUri] bool isNewService = false)
        {
            string submitEntityQuery = string.Empty;

            var service = submitEntityService.Service;

            try
            {
                if (submitEntityService.QueryType == QueryType.CustomQuery)
                {
                    string dropQuery = string.Format("IF OBJECT_ID('{0}.{1}', 'P') IS NOT NULL \n\t DROP PROCEDURE {0}.{1}; \nGO\n", "dbo", submitEntityService.StoredProcedureName);

                    submitEntityQuery = submitEntityService.CustomQuery;
                    string customQuery = dropQuery + submitEntityQuery;

                    DbUtil.ExecuteTransaction(customQuery);

                    service.Params = General.GetSpParams(submitEntityService.StoredProcedureName);
                }
                else
                {
                    string baseQuery = string.Empty;

                    if (submitEntityService.ActionType == ActionType.InsertAndUpdate)
                    {
                        baseQuery = FileUtil.GetFileContent(HttpContext.Current.Server.MapPath("~/DesktopModules/BusinessEngine/extensions/basic/services/sql-templates/submit-entity/insert&update-entity.sql"));
                        if (submitEntityService.BaseQuery == baseQuery) submitEntityService.BaseQuery = null;
                        submitEntityQuery = submitEntityService.BaseQuery != null ? submitEntityService.BaseQuery : baseQuery;

                        submitEntityService.InsertBaseQuery = null;
                        submitEntityService.UpdateBaseQuery = null;
                    }
                    else if (submitEntityService.ActionType == ActionType.Insert)
                    {
                        baseQuery = FileUtil.GetFileContent(HttpContext.Current.Server.MapPath("~/DesktopModules/BusinessEngine/extensions/basic/services/sql-templates/submit-entity/insert-entity.sql"));
                        if (submitEntityService.InsertBaseQuery == baseQuery) submitEntityService.InsertBaseQuery = null;
                        submitEntityQuery = submitEntityService.InsertBaseQuery != null ? submitEntityService.InsertBaseQuery : baseQuery;

                        submitEntityService.BaseQuery = null;
                        submitEntityService.UpdateBaseQuery = null;
                    }
                    if (submitEntityService.ActionType == ActionType.Update)
                    {
                        baseQuery = FileUtil.GetFileContent(HttpContext.Current.Server.MapPath("~/DesktopModules/BusinessEngine/extensions/basic/services/sql-templates/submit-entity/update-entity.sql"));
                        if (submitEntityService.UpdateBaseQuery == baseQuery) submitEntityService.UpdateBaseQuery = null;
                        submitEntityQuery = submitEntityService.UpdateBaseQuery != null ? submitEntityService.UpdateBaseQuery : baseQuery;

                        submitEntityService.BaseQuery = null;
                        submitEntityService.InsertBaseQuery = null;
                    }

                    var entity = submitEntityService.Entity;

                    var serviceParams = new List<ServiceParamInfo>();
                    var spParams = new List<string>();
                    var insertConditions = new List<string>();
                    var insertColumns = new List<string>();
                    var insertParams = new List<string>();
                    var updateConditions = new List<string>();
                    var updateParams = new List<string>();

                    //Insert Action
                    if (submitEntityService.ActionType == ActionType.InsertAndUpdate || submitEntityService.ActionType == ActionType.Insert)
                    {
                        //Insert Conditions
                        foreach (var group in entity.InsertConditions.GroupBy(f => f.GroupName))
                        {
                            var queryGroup = new List<string>();
                            foreach (var filter in group)
                            {
                                queryGroup.Add(filter.SqlQuery);
                            }

                            if (queryGroup.Count > 0) insertConditions.Add(string.Format("({0})", string.Join(" or ", queryGroup)));
                        }

                        //Insert Columns
                        foreach (var column in entity.InsertColumns.Where(c => c.IsSelected && !string.IsNullOrEmpty(c.ColumnValue)))
                        {
                            insertColumns.Add(column.ColumnName);
                        }

                        //Insert Params
                        foreach (var column in entity.InsertColumns.Where(c => c.IsSelected && !string.IsNullOrEmpty(c.ColumnValue)))
                        {
                            insertParams.Add(column.ColumnValue);
                        }
                    }

                    //Update Action
                    if (submitEntityService.ActionType == ActionType.InsertAndUpdate || submitEntityService.ActionType == ActionType.Update)
                    {
                        //Update Conditions
                        foreach (var group in entity.UpdateConditions.GroupBy(f => f.GroupName))
                        {
                            var queryGroup = new List<string>();
                            foreach (var filter in group)
                            {
                                queryGroup.Add(filter.SqlQuery);
                            }

                            if (queryGroup.Count > 0) updateConditions.Add(string.Format("({0})", string.Join(" or ", queryGroup)));
                        }

                        //Update Params
                        foreach (var column in entity.UpdateColumns.Where(c => c.IsSelected && !string.IsNullOrEmpty(c.ColumnValue)))
                        {
                            updateParams.Add(column.ColumnName + " = " + column.ColumnValue);
                        }
                    }

                    //Service Params
                    if (service.Params != null)
                    {
                        foreach (var serviceParam in service.Params)
                        {
                            spParams.Add(serviceParam.ParamName + " " + serviceParam.ParamType);
                        }
                    }

                    submitEntityQuery = submitEntityQuery.Replace("{InsertConditions}", string.Join(" and ", insertConditions));
                    submitEntityQuery = submitEntityQuery.Replace("{InsertColumns}", string.Join(",\n\t\t\t\t", insertColumns));
                    submitEntityQuery = submitEntityQuery.Replace("{InsertParams}", string.Join(",\n\t\t\t\t", insertParams));
                    submitEntityQuery = submitEntityQuery.Replace("{UpdateConditions}", string.Join(",\n\t\t\t\t", updateConditions));
                    submitEntityQuery = submitEntityQuery.Replace("{UpdateParams}", string.Join(",\n\t\t\t\t", updateParams));
                    submitEntityQuery = submitEntityQuery.Replace("{PrimaryKeyParam}", entity.PrimaryKeyParam);
                    submitEntityQuery = submitEntityQuery.Replace("{TableName}", entity.TableName);
                    submitEntityQuery = submitEntityQuery.Replace("{Schema}", "dbo");
                    submitEntityQuery = submitEntityQuery.Replace("{SpParams}", string.Join(",\n", spParams));
                    submitEntityQuery = submitEntityQuery.Replace("{ProcedureName}", submitEntityService.StoredProcedureName);

                    string dropQuery = string.Format("IF OBJECT_ID('{0}.{1}', 'P') IS NOT NULL \n\t DROP PROCEDURE {0}.{1}; \nGO\n", "dbo", submitEntityService.StoredProcedureName);

                    string customQuery = dropQuery + submitEntityQuery;

                    DbUtil.ExecuteTransaction(customQuery);

                    var objSubmitEntityServiceInfo = new SubmitEntityServiceInfo()
                    {
                        ItemID = submitEntityService.ItemID,
                        ServiceID = submitEntityService.ServiceID,
                        EntityID = submitEntityService.EntityID,
                        QueryType = submitEntityService.QueryType,
                        DatabaseObjectType = submitEntityService.DatabaseObjectType,
                        StoredProcedureName = submitEntityService.StoredProcedureName,
                        ActionType = submitEntityService.ActionType,
                        BaseQuery = submitEntityService.BaseQuery,
                        InsertBaseQuery = submitEntityService.InsertBaseQuery,
                        UpdateBaseQuery = submitEntityService.UpdateBaseQuery,
                        Entity = JsonConvert.SerializeObject(submitEntityService.Entity),
                        Settings = JsonConvert.SerializeObject(submitEntityService.Settings)
                    };

                    if (submitEntityService.ItemID == Guid.Empty)
                        submitEntityService.ItemID = SubmitEntityServiceRepository.AddSubmitEntityService(objSubmitEntityServiceInfo);
                    else
                        SubmitEntityServiceRepository.UpdateSubmitEntityService(objSubmitEntityServiceInfo);

                    General.SaveServiceParams(service.ServiceID, service.Params);
                }

                return Request.CreateResponse(HttpStatusCode.OK, submitEntityService.ItemID);
            }
            catch (Exception ex)
            {
                if (isNewService) ServiceRepository.Instance.DeleteService(service.ServiceID);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = ex.Message, Query = submitEntityQuery });
            }
        }

        #endregion

        #region Data Source Services

        [DnnAuthorize(StaticRoles = "Administrators")]
        [HttpGet]
        public HttpResponseMessage GetDataSourceService()
        {
            return GetDataSourceService(Guid.Empty);
        }

        [DnnAuthorize(StaticRoles = "Administrators")]
        [HttpGet]
        public HttpResponseMessage GetDataSourceService(Guid serviceID)
        {
            try
            {
                var dataSourceService = DataSourceServiceMapping.GetDataSourceServiceViewModel(serviceID);

                string baseQuery = FileUtil.GetFileContent(HttpContext.Current.Server.MapPath("~/DesktopModules/BusinessEngine/extensions/basic/services/sql-templates/data-source/data-source.sql"));

                dataSourceService = dataSourceService ?? new DataSourceServiceViewModel()
                {
                    BaseQuery = baseQuery,
                    QueryType = QueryType.QueryDesigner,
                    DatabaseObjectType = DatabaseObjectType.StoredProcedure,
                    Entities = Enumerable.Empty<ViewModels.Database.DataSource.EntityInfo>(),
                    Filters = Enumerable.Empty<FilterItemInfo>(),
                    SortItems = Enumerable.Empty<SortItemInfo>(),
                };

                if (string.IsNullOrEmpty(dataSourceService.BaseQuery)) dataSourceService.BaseQuery = baseQuery;

                if (dataSourceService.QueryType == QueryType.CustomQuery && !string.IsNullOrEmpty(dataSourceService.StoredProcedureName))
                {
                    dataSourceService.CustomQuery = General.GetSpScript(dataSourceService.StoredProcedureName);

                    dataSourceService.Service = new ServiceViewModel();
                    dataSourceService.Service.Params = General.GetSpParams(dataSourceService.StoredProcedureName);
                }

                return Request.CreateResponse(HttpStatusCode.OK, dataSourceService);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [DnnAuthorize(StaticRoles = "Administrators")]
        [HttpPost]
        public HttpResponseMessage CreateDataSourceService([FromBody] DataSourceServiceViewModel dataSourceService, [FromUri] bool isNewService = false)
        {
            string dataSourceQuery = string.Empty;

            var service = dataSourceService.Service;

            try
            {
                if (dataSourceService.QueryType == QueryType.CustomQuery)
                {
                    dataSourceQuery = dataSourceService.CustomQuery;

                    DbUtil.ExecuteSql(string.Format("IF OBJECT_ID('{0}.{1}', 'P') IS NOT NULL \n\t DROP PROCEDURE {0}.{1} \n", "dbo", dataSourceService.StoredProcedureName));

                    DbUtil.ExecuteSql(dataSourceQuery);
                }
                else
                {
                    string baseQuery = FileUtil.GetFileContent(HttpContext.Current.Server.MapPath("~/DesktopModules/BusinessEngine/Content/SqlTemplates/StoredProcedure/GetDataSource.sql"));
                    if (dataSourceService.BaseQuery == baseQuery) dataSourceService.BaseQuery = null;
                    dataSourceQuery = dataSourceService.BaseQuery != null ? dataSourceService.BaseQuery : baseQuery;

                    var spParams = new List<string>();
                    var selectedColumns = new List<string>();
                    var entities = new List<string>();
                    var filters = new List<string>();
                    var sortItems = new List<string>();

                    var pagingRegex = new Regex("(\\[STARTPAGING\\])(.*?)(\\[ENDPAGING\\])", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);

                    dataSourceQuery = dataSourceService.EnablePaging ? pagingRegex.Replace(dataSourceQuery, "\t$2") : pagingRegex.Replace(dataSourceQuery, "");

                    foreach (var property in dataSourceService.ViewModelProperties)
                    {
                        string value = string.Empty;

                        if (property.ValueType == "DataSource" && !string.IsNullOrEmpty(property.EntityAliasName) && !string.IsNullOrEmpty(property.ColumnName))
                            value = property.EntityAliasName + "." + property.ColumnName;
                        else if (property.ValueType == "Custom" && !string.IsNullOrEmpty(property.Value))
                            value = property.Value;

                        if (!string.IsNullOrEmpty(value)) selectedColumns.Add(string.Format("{0} as {1}", value, property.PropertyName));
                    }

                    //Service Params
                    if (service.Params != null)
                    {
                        foreach (var serviceParam in service.Params)
                        {
                            spParams.Add(string.Format("{0} {1} {2}", serviceParam.ParamName, serviceParam.ParamType, !string.IsNullOrEmpty(serviceParam.DefaultValue) ? (" = " + serviceParam.DefaultValue) : ""));
                        }
                    }

                    if (dataSourceService.JoinRelationships != null && dataSourceService.JoinRelationships.Any())
                    {
                        var existsEntities = new List<string>();

                        var firstJoin = dataSourceService.JoinRelationships.First();
                        
                        string itemss = string.Format(" dbo.[{0}] as {1} ", firstJoin.LeftEntityTableName, firstJoin.LeftEntityAliasName);

                        foreach (var relationship in dataSourceService.JoinRelationships ?? Enumerable.Empty<EntityJoinRelationInfo>())
                        {

                            //if (existsEntities.Contains(relationship.LeftEntityAliasName) == false)
                            //    itemss += string.Format(" dbo.[{0}] as {1} ", relationship.LeftEntityTableName, relationship.LeftEntityAliasName);

                            itemss += string.Format(" {0} dbo.{1} as {2} on {3} ", relationship.JoinType, relationship.RightEntityTableName, relationship.RightEntityAliasName, relationship.JoinConditions);

                            existsEntities.Add(relationship.LeftEntityAliasName);
                            existsEntities.Add(relationship.RightEntityAliasName);
                        }
                        entities.Add(itemss);
                    }
                    else
                    {
                        foreach (var entity in dataSourceService.Entities ?? Enumerable.Empty<ViewModels.Database.DataSource.EntityInfo>())
                        {
                            string item = string.Format(" dbo.[{0}] as {1} ", entity.TableName, entity.AliasName);

                            //if (entity.EnableJoin)
                            //{
                            //    foreach (var relationship in entity.JoinRelationships ?? Enumerable.Empty<EntityJoinRelationInfo>())
                            //    {
                            //        item += string.Format(" {0} {1} as {2} on {3} ", relationship.JoinType, relationship.RightEntityTableName, relationship.RightEntityAliasName, relationship.JoinConditions);

                            //        existsEntities.Add(relationship.RightEntityAliasName);
                            //    }
                            //}

                            entities.Add(item);
                        }
                    }

                    if (dataSourceService.Filters != null)
                    {
                        foreach (var group in dataSourceService.Filters.GroupBy(f => f.ConditionGroupName))
                        {
                            var queryGroup = new List<string>();
                            foreach (var filter in group)
                            {
                                if (filter.Type == 1) queryGroup.Add(filter.CustomQuery);
                            }

                            if (queryGroup.Count > 0) filters.Add(string.Format("({0})", string.Join(" or ", queryGroup)));
                        }
                    }

                    foreach (var sortItem in dataSourceService.SortItems ?? Enumerable.Empty<SortItemInfo>())
                    {
                        if (sortItem.Type == 0)
                        {
                            sortItems.Add(string.Format("{0}.[{1}] {2}", sortItem.EntityAliasName, sortItem.ColumnName, sortItem.SortType));
                        }
                        else if (sortItem.Type == 1)
                            sortItems.Add(sortItem.CustomColumn);
                    }

                    //Paging
                    if (dataSourceService.EnablePaging)
                    {
                        dataSourceQuery = dataSourceQuery.Replace("{PagingQuery}", "OFFSET (" + dataSourceService.PageIndexParam + " - 1) * " + dataSourceService.PageSizeParam + " ROWS FETCH NEXT " + dataSourceService.PageSizeParam + " ROWS ONLY;");
                    }
                    else
                    {
                        dataSourceQuery = dataSourceQuery.Replace("{PagingQuery}", string.Empty);
                    }

                    dataSourceQuery = dataSourceQuery.Replace("{Schema}", "dbo");
                    dataSourceQuery = dataSourceQuery.Replace("{ProcedureName}", dataSourceService.StoredProcedureName);
                    dataSourceQuery = dataSourceQuery.Replace("{SpParams}", string.Join(",\n", spParams));
                    dataSourceQuery = dataSourceQuery.Replace("{SelectedColumns}", string.Join(",", selectedColumns));
                    dataSourceQuery = dataSourceQuery.Replace("{Entities}", string.Join(",\n", entities));
                    dataSourceQuery = dataSourceQuery.Replace("{Filters}", filters.Any() ? "WHERE \n\t\t" + string.Join(" and\n\t\t", filters) : string.Empty);
                    dataSourceQuery = dataSourceQuery.Replace("{SortingQuery}", "ORDER BY \n\t\t" + string.Join(",", sortItems));
                    dataSourceQuery = dataSourceQuery.Replace("{TotalCountColumnName}", "[bEngine_TotalCount]");
                    dataSourceQuery = dataSourceQuery.Replace("{TotalCountAliasName}", "bEngine_tc");

                    string connectionString = "";

                    if (service.DatabaseID != null)
                    {
                        var database = DatabaseRepository.Instance.GetDatabase(service.DatabaseID.Value);
                        connectionString = database != null ? database.ConnectionString : "";
                    }

                    string dropQuery = string.Format("IF OBJECT_ID('{0}.{1}', 'P') IS NOT NULL \n\t DROP PROCEDURE {0}.{1}; \nGO\n", "dbo", dataSourceService.StoredProcedureName);

                    string query = dropQuery + dataSourceQuery;

                    DbUtil.ExecuteTransaction(query);

                    var objDataSourceServiceInfo = new DataSourceServiceInfo()
                    {
                        ItemID = dataSourceService.ItemID,
                        ServiceID = dataSourceService.ServiceID,
                        ViewModelID = dataSourceService.ViewModelID,
                        QueryType = dataSourceService.QueryType,
                        DatabaseObjectType = dataSourceService.DatabaseObjectType,
                        StoredProcedureName = dataSourceService.StoredProcedureName,
                        BaseQuery = dataSourceService.BaseQuery,
                        Entities = JsonConvert.SerializeObject(dataSourceService.Entities),
                        JoinRelationships = JsonConvert.SerializeObject(dataSourceService.JoinRelationships),
                        ViewModelProperties = JsonConvert.SerializeObject(dataSourceService.ViewModelProperties),
                        Filters = JsonConvert.SerializeObject(dataSourceService.Filters),
                        SortItems = JsonConvert.SerializeObject(dataSourceService.SortItems),
                        EnablePaging = dataSourceService.EnablePaging,
                        PageIndexParam = dataSourceService.PageIndexParam,
                        PageSizeParam = dataSourceService.PageSizeParam,
                        Settings = JsonConvert.SerializeObject(dataSourceService.Settings)
                    };

                    if (dataSourceService.ItemID == Guid.Empty)
                        dataSourceService.ItemID = DataSourceServiceRepository.AddDataSourceService(objDataSourceServiceInfo);
                    else
                        DataSourceServiceRepository.UpdateDataSourceService(objDataSourceServiceInfo);

                    General.SaveServiceParams(service.ServiceID, service.Params);
                }

                return Request.CreateResponse(HttpStatusCode.OK, dataSourceService.ItemID);
            }
            catch (Exception ex)
            {
                if (isNewService) ServiceRepository.Instance.DeleteService(service.ServiceID);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = ex.Message, Query = dataSourceQuery });
            }
        }

        #endregion

        #endregion

        #region Temp Methods

        [AllowAnonymous]
        [HttpPost]
        public async Task<HttpResponseMessage> UploadExcel()
        {
            try
            {
                var currentRequest = HttpContext.Current.Request;

                if (HttpContext.Current.Request.Files.Count > 0)
                {
                    var fileName = HttpContext.Current.Request.Files[0].FileName;
                    if (!Host.AllowedExtensionWhitelist.AllowedExtensions.Contains(Path.GetExtension(fileName)))
                        throw new Exception("File type not allowed");
                }

                if (Request.Content.IsMimeMultipartContent())
                {
                    string mapPath = PortalSettings.HomeSystemDirectoryMapPath + "BusinessEngine/Temp/Excels/";
                    if (!Directory.Exists(mapPath)) Directory.CreateDirectory(mapPath);

                    string columns = HttpContext.Current.Request.Params["Columns"];

                    var streamProvider = new CustomMultipartFormDataStreamProviderChangeFileName(mapPath);

                    await Request.Content.ReadAsMultipartAsync(streamProvider);

                    var file = streamProvider.FileData[0];
                    var result = ImportExcelTemp.PopulateExcelData(file.LocalFileName, columns.Split(',').ToList());

                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                    throw new HttpResponseException(response);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        #endregion
    }
}
