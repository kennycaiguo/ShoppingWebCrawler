﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 【国美】搜索页面 参数
    /// 具体详细的搜索面板 回头慢慢完善
    /// </summary>
    public sealed class GuomeiFetchWebPageArgument : BaseFetchWebPageArgument
    {

        public GuomeiFetchWebPageArgument()
        {
            this.Platform = SupportPlatformEnum.Guomei;
        }

        /// <summary>
        /// 排序参数
        /// </summary>
        public override string OrderFiledName
        {
            get
            {
                return "sort";
            }


        }


        /// <summary>
        /// 获取【国美】平台支持的排序字段列表
        /// </summary>
        /// <returns></returns>
        public override List<OrderField> GetCurrentPlatformSupportOrderFields()
        {
            List<OrderField> fields = new List<OrderField>() {

                 new OrderField { DisplayName="综合", FieldValue="00" },
                 new OrderField { DisplayName="销量", FieldValue="10" },
                 new OrderField { DisplayName="价格降序", FieldValue="20" },
                 new OrderField { DisplayName="价格升序", FieldValue="21" },
                 new OrderField { DisplayName="评价", FieldValue="50" },
                 new OrderField { DisplayName="新品", FieldValue="30" },
                
            };

            return fields;
        }
    }
}
