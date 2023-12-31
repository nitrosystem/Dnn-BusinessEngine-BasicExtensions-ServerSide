﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels.Database.Public
{
    public class SortItemInfo
    {
        public int Type { get; set; }
        public string EntityAliasName { get; set; }
        public string ColumnName { get; set; }
        public string CustomColumn { get; set; }
        public string SortType { get; set; }
    }
}