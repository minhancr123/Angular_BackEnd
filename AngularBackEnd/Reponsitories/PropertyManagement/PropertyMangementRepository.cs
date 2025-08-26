using AngularBackEnd.Models.InventoryManagement;
using AngularBackEnd.Models.PropertyManagement;
using AngularBackEnd.Reponsitories.PropertyMangement;
using DpsLibs.Data;
using JeeBeginner.Classes;
using JeeBeginner.Models.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Data;

namespace AngularBackEnd.Reponsitories.InventoryMangement
{
    public class PropertyMangementRepository : IPropertyMangementRepository
    {
        private readonly string _connectionString;

        public PropertyMangementRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //Property Type
        private Hashtable InitDataPropertyType(PropertyType model, long userId, bool? isUpdate = false)
        {
            Hashtable val = new Hashtable();

            void AddIfValid(string key, object? value)
            {
                if (value != null && value != DBNull.Value)
                {
                    if (!val.ContainsKey(key))
                        val.Add(key, value);
                }
            }

            AddIfValid("MaLoai", model.MaLoai);
            AddIfValid("TenLoai", model.TenLoai);
            AddIfValid("TrangThai", model.TrangThai.HasValue ? (model.TrangThai.Value ? 1 : 0) : (object?)null);


            return val;
        }


        public async Task<IEnumerable<PropertyType>> GetPropertyTypeList(SqlConditions conds, string? orderByStr = null)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                DataTable dt = new DataTable();
                string sql;

                if (string.IsNullOrWhiteSpace(orderByStr))
                    orderByStr = "TenLoai ASC";

                if (conds != null && conds.Count > 0)
                {
                    sql = $@"
            SELECT * FROM TS_DM_LoaiTS
            WHERE 1=1 AND (where)
            ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql, "(where)", conds);
                }
                else
                {
                    sql = $@"
            SELECT * FROM TS_DM_LoaiTS
            ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql);
                }

                var result = dt.AsEnumerable().Select(row => new PropertyType
                {
                    IdLoaiTS = Convert.ToInt64(row["IdLoaiTS"]),
                    MaLoai = row["MaLoai"]?.ToString(),
                    TenLoai = row["TenLoai"]?.ToString(),
                    TrangThai = row["TrangThai"] != DBNull.Value ? Convert.ToBoolean(row["TrangThai"]) : (bool?)null
                });

                return await Task.FromResult(result);
            }
        }

        public async Task<ReturnSqlModel> UpdatePropertyType(PropertyType propertyTypeModel, long ModifiedBy)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                SqlConditions conds = new SqlConditions();
                conds.Add("IdLoaiTS", propertyTypeModel.IdLoaiTS);

                var val = InitDataPropertyType(propertyTypeModel, ModifiedBy, true);

                try
                {
                    int x = cnn.Update(val, conds, "TS_DM_LoaiTS");
                    if (x <= 0)
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                }
                catch (Exception ex)
                {
                    cnn.RollbackTransaction();
                    cnn.EndTransaction();
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }
            return await Task.FromResult(new ReturnSqlModel());
        }

        public async Task<ReturnSqlModel> CreatePropertyType(PropertyType propertyTypeModel, long CreatedBy)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var val = InitDataPropertyType(propertyTypeModel, CreatedBy);
                    int x = cnn.Insert(val, "TS_DM_LoaiTS");
                    if (x <= 0)
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_EXCEPTION));
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }
            return await Task.FromResult(new ReturnSqlModel());
        }

        public async Task<ReturnSqlModel> DeletePropertyType(int id, int userId)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                SqlConditions conds = new SqlConditions();
                conds.Add("IdLoaiTS", id, SqlOperator.Equals);

                var result = cnn.Delete(conds, "TS_DM_LoaiTS");

                if (result <= 0)
                    return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));

                return await Task.FromResult(new ReturnSqlModel());
            }
        }

        //Group Property Type

        private Hashtable InitDataGroupPropertyType(GroupPropertyType model, long userId, bool? isUpdate = false)
        {
            Hashtable val = new Hashtable();

            void AddIfValid(string key, object? value)
            {
                if (value != null && value != DBNull.Value)
                {
                    if (!val.ContainsKey(key))
                        val.Add(key, value);
                }
            }

            AddIfValid("MaNhom", model.MaNhom);
            AddIfValid("TenNhom", model.TenNhom);
            AddIfValid("TrangThai", model.TrangThai);

           

            return val;
        }

        public async Task<IEnumerable<GroupPropertyType>> GetGroupPropertyTypeList(SqlConditions conds, string? orderByStr = null)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                DataTable dt = new DataTable();
                string sql;

                if (string.IsNullOrWhiteSpace(orderByStr))
                    orderByStr = "TenNhom ASC";

                if (conds != null && conds.Count > 0)
                {
                    sql = $@"
                SELECT *
                FROM TS_DM_PhanNhomTS
                WHERE 1=1 AND (where)
                ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql, "(where)", conds);
                }
                else
                {
                    sql = $@"
                SELECT *
                FROM TS_DM_PhanNhomTS
                ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql);
                }

                var result = dt.AsEnumerable().Select(row => new GroupPropertyType
                {
                    IdPNTS = Convert.ToInt64(row["IdPNTS"]),
                    MaNhom = row["MaNhom"]?.ToString(),
                    TenNhom = row["TenNhom"]?.ToString(),
                    TrangThai = row["TrangThai"] != DBNull.Value ? (bool?)Convert.ToBoolean(row["TrangThai"]) : null
                });

                return await Task.FromResult(result);
            }
        }

        public async Task<ReturnSqlModel> UpdateGroupPropertyType(GroupPropertyType groupPropertyTypeModel, long ModifiedBy)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    SqlConditions conds = new SqlConditions();
                    conds.Add("IdPNTS", groupPropertyTypeModel.IdPNTS);

                    var val = InitDataGroupPropertyType(groupPropertyTypeModel, ModifiedBy, true);
                    int x = cnn.Update(val, conds, "TS_DM_PhanNhomTS");

                    if (x <= 0)
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                }
                catch (Exception ex)
                {
                    cnn.RollbackTransaction();
                    cnn.EndTransaction();
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }
            return await Task.FromResult(new ReturnSqlModel());
        }

        public async Task<ReturnSqlModel> CreateGroupPropertyType(GroupPropertyType groupPropertyTypeModel, long CreatedBy)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var val = InitDataGroupPropertyType(groupPropertyTypeModel, CreatedBy);
                    int x = cnn.Insert(val, "TS_DM_PhanNhomTS");
                    if (x <= 0)
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }
            return await Task.FromResult(new ReturnSqlModel());
        }

        public async Task<ReturnSqlModel> DeleteGroupPropertyType(int id, int userId)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                SqlConditions conds = new SqlConditions();
                conds.Add("IdPNTS", id, SqlOperator.Equals);

                var result = cnn.Delete(conds, "TS_DM_PhanNhomTS");
                if (result <= 0)
                    return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));

                return await Task.FromResult(new ReturnSqlModel());
            }
        }

        //Ly do tang giam
        private Hashtable InitDataReason(Reason model, long userId, bool isUpdate = false)
        {
            Hashtable val = new Hashtable();

            void AddIfValid(string key, object? value)
            {
                if (value != null && value != DBNull.Value)
                {
                    if (!val.ContainsKey(key))
                        val.Add(key, value);
                }
            }

            AddIfValid("LoaiTangGiam", model.LoaiTangGiam);
            AddIfValid("MaTangGiam", model.MaTangGiam);
            AddIfValid("TenTangGiam", model.TenTangGiam);
            AddIfValid("TrangThai", model.TrangThai);


            return val;
        }

        public async Task<IEnumerable<Reason>> GetReasonList(SqlConditions conds, string? orderByStr = null)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                DataTable dt = new DataTable();
                string sql;

                if (string.IsNullOrWhiteSpace(orderByStr))
                    orderByStr = "TenTangGiam ASC";

                if (conds != null && conds.Count > 0)
                {
                    sql = $@"
                SELECT *
                FROM TS_DM_LyDoTangGiamTS
                WHERE 1=1 AND (where)
                ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql, "(where)", conds);
                }
                else
                {
                    sql = $@"
                SELECT *
                FROM TS_DM_LyDoTangGiamTS
                ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql);
                }

                var result = dt.AsEnumerable().Select(row => new Reason
                {
                    IdRow = Convert.ToInt64(row["IdRow"]),
                    LoaiTangGiam = Convert.ToInt64(row["LoaiTangGiam"]),
                    MaTangGiam = row["MaTangGiam"]?.ToString(),
                    TenTangGiam = row["TenTangGiam"]?.ToString(),
                    TrangThai = row["TrangThai"] != DBNull.Value ? (bool?)Convert.ToBoolean(row["TrangThai"]) : null
                });

                return await Task.FromResult(result);
            }
        }

        public async Task<ReturnSqlModel> UpdateReason(Reason reasonModel, long ModifiedBy)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    SqlConditions conds = new SqlConditions();
                    conds.Add("IdRow", reasonModel.IdRow);

                    var val = InitDataReason(reasonModel, ModifiedBy, true);
                    int x = cnn.Update(val, conds, "TS_DM_LyDoTangGiamTS");

                    if (x <= 0)
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                }
                catch (Exception ex)
                {
                    cnn.RollbackTransaction();
                    cnn.EndTransaction();
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }

            return await Task.FromResult(new ReturnSqlModel());
        }

        public async Task<ReturnSqlModel> CreateReason(Reason reasonModel, long CreatedBy)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var val = InitDataReason(reasonModel, CreatedBy);
                    int x = cnn.Insert(val, "TS_DM_LyDoTangGiamTS");

                    if (x <= 0)
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }

            return await Task.FromResult(new ReturnSqlModel());
        }

        public async Task<ReturnSqlModel> DeleteReason(int id, int userId)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                SqlConditions conds = new SqlConditions();
                conds.Add("IdRow", id, SqlOperator.Equals);

                var result = cnn.Delete(conds, "TS_DM_LyDoTangGiamTS");

                if (result <= 0)
                    return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));

                return await Task.FromResult(new ReturnSqlModel());
            }
        }
    }
}
