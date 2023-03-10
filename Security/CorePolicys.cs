using System;
using System.Collections.Generic;
using System.Reflection;

namespace EnergyScanApi.Security
{
    public class CorePolicys
    {
        //***Every const string will be a Policy in the Database!***

        public const string create = "create";
        public const string delete = "delete";
        public const string read = "read";
        public const string update = "update";

        public const string groupmanager = "groupmanager";
        public const string policymanager = "policymanager";
        public const string requestmanager = "requestmanager";
        public const string usermanager = "usermanager";

        public List<string> GetPolicies()
        {
            List<string> policies = new List<string>();
            Type coretype = typeof(CorePolicys);
            FieldInfo[] constfields = coretype.GetFields();
            for (int i = 0; i < constfields.Length; i++)
            {
                if (constfields[i].IsLiteral && constfields[i].FieldType.Equals(typeof(string)))
                {
                    string s = (string)constfields[i].GetValue(null);
                    policies.Add(s);
                }
            }
            return policies;
        }
    }
}
