﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels.Database.Public
{
    public class EntityJoinRelationInfo
    {
        public string JoinType { get; set; }
        public string LeftEntityAliasName { get; set; }
        public string LeftEntityTableName { get; set; }
        public string RightEntityAliasName { get; set; }
        public string RightEntityTableName { get; set; }
        public string JoinConditions { get; set; }
    }
}