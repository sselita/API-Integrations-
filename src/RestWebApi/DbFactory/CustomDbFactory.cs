using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using System.Reflection;
using RestWebApi.Services.Helpers;
using System.Linq;

namespace RestWebApi.DbFactory
{
    public class CustomDbFactory : ICustomDbFactory
    {
        /// <summary>
        /// /
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<T> PrepareMasterDataGroupObject<T>(SqlDataReader reader) where T : new()
        {
            var fieldNames = GetAllFieldNamesAndTypeFromSqlDataReader(reader);
            Type t = typeof(T);
            var listOfT = (List<T>)CreateGenericListOfT(t);
            while (reader.Read())
            {
                T tInstance = new T();
                foreach (var field in fieldNames)
                {

                    switch (field.Item2)
                    {
                        case "nvarchar":
                            {
                                SetPropertyValue<T>(field.Item1, reader.SafeGetString(field.Item1.Trim()), tInstance);
                                break;
                            }
                        case "varchar":
                            {
                                SetPropertyValue<T>(field.Item1, reader.SafeGetString(field.Item1.Trim()), tInstance);
                                break;
                            }
                        case "int":
                            {
                                SetPropertyValue<T>(field.Item1, reader.SafeGetInt(field.Item1.Trim()), tInstance);
                                break;
                            }  
                        case "tinyint":
                            {
                                SetPropertyValue<T>(field.Item1, reader.SafeGetByte(field.Item1.Trim()), tInstance);
                                break;
                            }
                        case "datetime":
                            {
                                SetPropertyValue<T>(field.Item1, reader.SafeGetDateTime(field.Item1.Trim()), tInstance);
                                break;
                            }
                        case "decimal":
                            {
                                SetPropertyValue<T>(field.Item1, reader.SafeGetDecimal(field.Item1.Trim()), tInstance);
                                break;
                            }
                        case "byte":
                            {
                                SetPropertyValue<T>(field.Item1, reader.SafeGetByte(field.Item1.Trim()), tInstance);
                                break;
                            }
                        default:
                            break;
                    }
                }
                listOfT.Add(tInstance);
            }
            return listOfT;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private List<(string, string)> GetAllFieldNamesAndTypeFromSqlDataReader(SqlDataReader reader)
        {
            var columns = new List<(string, string)>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add((reader.GetName(i), reader.GetDataTypeName(i)));
            }
            return columns;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public object CreateGenericListOfT(Type t)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(t);
            var instance = Activator.CreateInstance(constructedListType);
            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        /// <param name="obj"></param>
        public void SetPropertyValue<T>(string propName, dynamic value, object obj)
        {
            Type t = typeof(T);
            // var propInfo = typeof(obj).GetEnumValues();
            PropertyInfo propertyInfo = t.GetProperty(propName.Replace(" ", ""));
            if (propertyInfo != null)
                propertyInfo.SetValue(obj, Convert.ChangeType(value, propertyInfo.PropertyType), null);    
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outobj"></param>
        /// <param name="baseobj"></param>
        //public void MappQuoteProperty(out List<ApiQuote> outobjs, List<ApiGetQuoteResult> baseobj) 
        //{
        //    outobjs = new List<ApiQuote>();
        //    var headers = baseobj.GroupBy(x => x.No).Select(x => x.First()).ToList();

        //    ApiQuote outobj = null;
        //    foreach (var a in headers)
        //    {
               
        //            outobj = new ApiQuote();
        //            outobj.No = a.No;
        //            outobj.Description = a.Description;
        //            outobj.CustomerNo = a.CustomerNo;
        //            outobj.CurrencyCode = a.CurrencyCode;
        //            outobj.CurrencyRate = a.CurrencyRate;
        //            outobj.OrderDateTime = a.OrderDateTime;
        //            outobj.POSCode = a.POSCode;
        //            outobj.PaymentMethod = a.PaymentMethod;
        //            outobj.DocumentStatusTypePOS = a.DocumentStatusTypePOS;
        //            outobj.FromPos = a.FromPos;
        //            outobj.POSCode = a.POSCode;
        //        outobj.LastDateTimeModified = a.LastDateTimeModified;

        //        var lines = baseobj.Where(x => x.DocumentNo.Equals(a.No)).ToList();
        //        foreach (var l in lines)
        //        {
        //            outobj.ApiQuoteLines.Add(new ApiQuoteLine
        //            {
        //                DocumentNo = l.DocumentNo,
        //                ItemNo = l.ItemNo,
        //                Quantity = l.Quantity,
        //                VatCode = l.VatCode,
        //                AmountWithVat = l.AmountWithVat,
        //                AmountWithoutVat = l.AmountWithoutVat,
        //                LineDiscountPercentage = l.LineDiscountPercentage,
        //                UnitOfMeasure = l.UnitOfMeasure
        //            });
        //        }
        //        outobjs.Add(outobj);
        //    }

        //}

    }
}
