﻿using NitroSystem.Dnn.BusinessEngine.Data.Entities.Tables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels.Database.Public
{
    public class EntityInfo
    {
        public Guid EntityID { get; set; }
        public string EntityName { get; set; }
        public string AliasName { get; set; }
        public string TableName { get; set; }
        public bool EnableJoin { get; set; }
        public IEnumerable<EntityJoinRelationInfo> JoinRelationships { get; set; }
    }
}