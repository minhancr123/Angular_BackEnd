using AngularBackEnd.Models.InventoryManagement;
using DpsLibs.Data;
using JeeBeginner.Classes;
using JeeBeginner.Models.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Data;

namespace AngularBackEnd.Reponsitories.InventoryMangement
{
    public class IventoryMangementRepository : IInventoryMangementRepository
    {
        private readonly string _connectionString;

        public IventoryMangementRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //ItemType
        public async Task<IEnumerable<ItemTypeDTO>> GetItemTypeList(SqlConditions conds, string? orderByStr)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                DataTable dt = new DataTable();
                string sql;

                if (string.IsNullOrWhiteSpace(orderByStr))
                    orderByStr = "TenLMH ASC";

                if (conds != null && conds.Count > 0)
                {
                    // Nếu có điều kiện lọc => thêm (where)
                    sql = $@"
                    SELECT *
                    FROM DM_LoaiMatHang
                    WHERE 1=1 AND (where)
                    ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql, "(where)", conds);
                }
                else
                {
                    // Nếu không có điều kiện => không cần (where)
                    sql = $@"
                    SELECT *
                    FROM DM_LoaiMatHang
                    ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql);
                }


                var result = dt.AsEnumerable().Select(row => new ItemTypeDTO
                {
                    IdLMH = Convert.ToInt32(row["IdLMH"]),
                    MaLMH = row["MaLMH"]?.ToString(),
                    TenLMH = row["TenLMH"]?.ToString() ?? string.Empty,
                    IdCustomer = row["IdCustomer"] != DBNull.Value ? (int?)Convert.ToInt32(row["IdCustomer"]) : null,
                    IdLMHParent = row["IdLMHParent"] != DBNull.Value ? (int?)Convert.ToInt32(row["IdLMHParent"]) : null,
                    Mota = row["Mota"]?.ToString(),
                    HinhAnh = row["HinhAnh"]?.ToString(),
                    DoUuTien = row["DoUuTien"] != DBNull.Value ? (int?)Convert.ToInt32(row["DoUuTien"]) : null,
                    isDel = Convert.ToBoolean(row["isDel"]),
                    //CreatedBy = row["CreatedBy"] != DBNull.Value ? (int?)Convert.ToInt32(row["CreatedBy"]) : null,
                    //DeleteBy = row["DeleteBy"] != DBNull.Value ? (int?)Convert.ToInt32(row["DeleteBy"]) : null,
                    //ModifiedBy = row["ModifiedBy"] != DBNull.Value ? (int?)Convert.ToInt32(row["ModifiedBy"]) : null,
                    CreatedDate = row["CreatedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["CreatedDate"]) : null,
                    //ModifiedDate = row["ModifiedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["ModifiedDate"]) : null,
                    //DeleteDate = row["DeleteDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["DeleteDate"]) : null,
                    IdKho = row["IdKho"] != DBNull.Value ? (int?)Convert.ToInt32(row["IdKho"]) : null,
                });

                return await Task.FromResult(result);
            }
        }
       


        private Hashtable InitDataItemType(ItemTypeModel item, long ModifiedBy, bool isUpdate = false)
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

            AddIfValid("MaLMH", item.MaLMH);
            AddIfValid("TenLMH", item.TenLMH);
            AddIfValid("IdCustomer", item.IdCustomer);
            AddIfValid("IdLMHParent", item.IdLMHParent);
            AddIfValid("Mota", item.Mota);
            AddIfValid("HinhAnh", item.HinhAnh);
            AddIfValid("DoUuTien", item.DoUuTien);
            AddIfValid("IdKho", item.IdKho);
            AddIfValid("isDel", item.isDel ? 1 : 0); // luôn có giá trị

            if (!isUpdate)
            {
                AddIfValid("CreatedBy", ModifiedBy);
                AddIfValid("CreatedDate", DateTime.UtcNow);
            }
            else
            {
                AddIfValid("ModifiedBy", ModifiedBy);
                AddIfValid("ModifiedDate", DateTime.UtcNow);
            }

            return val;
        }

       




        public async Task<ReturnSqlModel> UpdateItemType(ItemTypeModel itemTypeModel, long ModifiedBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using(DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("IdLMH", itemTypeModel.IdLMH);
                    val = InitDataItemType(itemTypeModel, ModifiedBy, true);

                    int x = cnn.Update(val, conds, "DM_LoaiMatHang");

                    if(x <= 0)
                    {
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                    }

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

        //Item
        public async Task<IEnumerable<ItemDTO>> GetItemList(SqlConditions conds, string? orderByStr = null)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                DataTable dt = new DataTable();
                string sql;

                if (string.IsNullOrWhiteSpace(orderByStr))
                    orderByStr = "TenMatHang ASC";

                if (conds != null && conds.Count > 0)
                {
                    sql = $@"
                        SELECT *
                        FROM DM_MatHang
                        WHERE 1=1 AND (where)
                        ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql, "(where)", conds);
                }
                else
                {
                    sql = $@"
                    SELECT *
                    FROM DM_MatHang
                    ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql);
                }

                var result = dt.AsEnumerable().Select(row => new ItemDTO
                {
                    IdMH = Convert.ToInt32(row["IdMH"]),
                    MaHang = row["MaHang"]?.ToString(),
                    TenMatHang = row["TenMatHang"]?.ToString(),
                    IdLMH = row["IdLMH"] != DBNull.Value ? (int?)Convert.ToInt32(row["IdLMH"]) : null,
                    IdDVT = row["IdDVT"] != DBNull.Value ? (int?)Convert.ToInt32(row["IdDVT"]) : null,
                    Mota = row["Mota"]?.ToString(),
                    GiaMua = row["GiaMua"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["GiaMua"]) : null,
                    GiaBan = row["GiaBan"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["GiaBan"]) : null,
                    //CreatedBy = row["CreatedBy"] != DBNull.Value ? (int?)Convert.ToInt32(row["CreatedBy"]) : null,
                    CreatedDate = row["CreatedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["CreatedDate"]) : null,
                    //IsDel = row["IsDel"] != DBNull.Value && Convert.ToBoolean(row["IsDel"]),
                    Barcode = row["Barcode"]?.ToString(),
                    NgungKinhDoanh = row["NgungKinhDoanh"] != DBNull.Value && Convert.ToBoolean(row["NgungKinhDoanh"]),
                    TenOnSite = row["TenOnSite"]?.ToString(),
                    HinhAnh = row["HinhAnh"]?.ToString(),
                    ChiTietMoTa = row["ChiTietMoTa"]?.ToString(),
                    KichThuoc = row["KichThuoc"]?.ToString(),
                    ThongSo = row["ThongSo"]?.ToString(),
                    TheoDoiTonKho = row["TheoDoiTonKho"] != DBNull.Value && Convert.ToBoolean(row["TheoDoiTonKho"]),
                    TheodoiLo = row["TheodoiLo"] != DBNull.Value && Convert.ToBoolean(row["TheodoiLo"]),
                    MaLuuKho = row["MaLuuKho"]?.ToString(),
                    MaViTriKho = row["MaViTriKho"]?.ToString(),
                    SoKyTinhKhauHaoToiThieu = row["SoKyTinhKhauHaoToiThieu"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["SoKyTinhKhauHaoToiThieu"]?.ToString())
        ? (int?)Convert.ToInt32(row["SoKyTinhKhauHaoToiThieu"]) : null,
                    SoKyTinhKhauHaoToiDa = row["SoKyTinhKhauHaoToiDa"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["SoKyTinhKhauHaoToiDa"]?.ToString())
        ? (int?)Convert.ToInt32(row["SoKyTinhKhauHaoToiDa"]) : null,
                    IsTaiSan = row["IsTaiSan"] != DBNull.Value && Convert.ToBoolean(row["IsTaiSan"])
                    // Bạn có thể thêm các trường khác nếu cần như VAT, IdNhanHieu, IdXuatXu, ...
                });

                return await Task.FromResult(result);
            }
        }

        public async Task<IEnumerable<ItemDTO>> GetItemById(string id)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                DataTable dt = new DataTable();
                string sql = @"
            SELECT *
            FROM DM_MatHang
            WHERE IdMH = @IdMH";

                var conds = new SqlConditions();
                conds.Add("IdMH", id, SqlOperator.Equals);

                dt = cnn.CreateDataTable(sql, conds); // ✅ Đã sửa ở đây

                var result = dt.AsEnumerable().Select(row => new ItemDTO
                {
                    IdMH = Convert.ToInt32(row["IdMH"]),
                    MaHang = row["MaHang"]?.ToString(),
                    TenMatHang = row["TenMatHang"]?.ToString(),
                    IdLMH = row["IdLMH"] != DBNull.Value ? (int?)Convert.ToInt32(row["IdLMH"]) : null,
                    IdDVT = row["IdDVT"] != DBNull.Value ? (int?)Convert.ToInt32(row["IdDVT"]) : null,
                    Mota = row["Mota"]?.ToString(),
                    GiaMua = row["GiaMua"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["GiaMua"]) : null,
                    GiaBan = row["GiaBan"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["GiaBan"]) : null,
                    Barcode = row["Barcode"]?.ToString(),
                    NgungKinhDoanh = row["NgungKinhDoanh"] != DBNull.Value && Convert.ToBoolean(row["NgungKinhDoanh"]),
                    CreatedDate = row["CreatedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["CreatedDate"]) : null,
                    TenOnSite = row["TenOnSite"]?.ToString(),
                    HinhAnh = row["HinhAnh"]?.ToString(),
                    ChiTietMoTa = row["ChiTietMoTa"]?.ToString(),
                    KichThuoc = row["KichThuoc"]?.ToString(),
                    ThongSo = row["ThongSo"]?.ToString(),
                    TheoDoiTonKho = row["TheoDoiTonKho"] != DBNull.Value && Convert.ToBoolean(row["TheoDoiTonKho"]),
                    TheodoiLo = row["TheodoiLo"] != DBNull.Value && Convert.ToBoolean(row["TheodoiLo"]),
                    MaLuuKho = row["MaLuuKho"]?.ToString(),
                    MaViTriKho = row["MaViTriKho"]?.ToString(),
                    SoKyTinhKhauHaoToiThieu = row["SoKyTinhKhauHaoToiThieu"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["SoKyTinhKhauHaoToiThieu"]?.ToString())
                        ? (int?)Convert.ToInt32(row["SoKyTinhKhauHaoToiThieu"]) : null,
                    SoKyTinhKhauHaoToiDa = row["SoKyTinhKhauHaoToiDa"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["SoKyTinhKhauHaoToiDa"]?.ToString())
                        ? (int?)Convert.ToInt32(row["SoKyTinhKhauHaoToiDa"]) : null,
                    IsTaiSan = row["IsTaiSan"] != DBNull.Value && Convert.ToBoolean(row["IsTaiSan"])
                });

                return await Task.FromResult(result);
            }
        }

        public Hashtable InitDataItem(ItemModel item, long modifiedBy, bool isUpdate = false)
        {
            Hashtable val = new Hashtable();

            void AddIfValid(string key, object? value)
            {
                if (value != null && value != DBNull.Value)
                {
                    val.Add(key, value);
                }
            }

            AddIfValid("MaHang", item.MaHang);
            AddIfValid("TenMatHang", item.TenMatHang);
            AddIfValid("IdLMH", item.IdLMH);
            AddIfValid("IdDVT", item.IdDVT);
            AddIfValid("Mota", item.Mota);
            AddIfValid("GiaMua", item.GiaMua);
            AddIfValid("GiaBan", item.GiaBan);
            AddIfValid("VAT", item.VAT);
            AddIfValid("Barcode", item.Barcode);
            AddIfValid("NgungKinhDoanh", item.NgungKinhDoanh.HasValue ? (item.NgungKinhDoanh.Value ? 1 : 0) : (object?)null);
            AddIfValid("IdDVTCap2", item.IdDVTCap2);
            AddIfValid("QuyDoiDVTCap2", item.QuyDoiDVTCap2);
            AddIfValid("IdDVTCap3", item.IdDVTCap3);
            AddIfValid("QuyDoiDVTCap3", item.QuyDoiDVTCap3);
            AddIfValid("TenOnSite", item.TenOnSite);
            AddIfValid("IdNhanHieu", item.IdNhanHieu);
            AddIfValid("IdXuatXu", item.IdXuatXu);
            AddIfValid("HinhAnh", item.HinhAnh);
            AddIfValid("ChiTietMoTa", item.ChiTietMoTa);
            AddIfValid("MaPhu", item.MaPhu);
            AddIfValid("KichThuoc", item.KichThuoc);
            AddIfValid("ThongSo", item.ThongSo);
            AddIfValid("TheoDoiTonKho", item.TheoDoiTonKho.HasValue ? (item.TheoDoiTonKho.Value ? 1 : 0) : (object?)null);
            AddIfValid("TheodoiLo", item.TheodoiLo.HasValue ? (item.TheodoiLo.Value ? 1 : 0) : (object?)null);
            AddIfValid("MaLuuKho", item.MaLuuKho);
            AddIfValid("MaViTriKho", item.MaViTriKho);
            AddIfValid("UpperLimit", item.UpperLimit);
            AddIfValid("LowerLimit", item.LowerLimit);
            AddIfValid("IsTaiSan", item.IsTaiSan.HasValue ? (item.IsTaiSan.Value ? 1 : 0) : (object?)null);
            AddIfValid("SoKyTinhKhauHaoToiThieu", item.SoKyTinhKhauHaoToiThieu);
            AddIfValid("SoKyTinhKhauHaoToiDa", item.SoKyTinhKhauHaoToiDa);
            AddIfValid("SoNamDeNghi", item.SoNamDeNghi);
            AddIfValid("TiLeHaoMon", item.TiLeHaoMon);
            AddIfValid("IsDel", item.IsDel.HasValue ? (item.IsDel.Value ? 1 : 0) : (object?)null);

            if (!isUpdate)
            {
                AddIfValid("CreatedBy", modifiedBy);
                AddIfValid("CreatedDate", DateTime.UtcNow);
            }
            else
            {
                AddIfValid("ModifiedBy", modifiedBy);
                AddIfValid("ModifiedDate", DateTime.UtcNow);
            }

            return val;
        }




        public async Task<ReturnSqlModel> CreateItemType(ItemTypeModel itemTypeModel, long CreatedBy)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var val = InitDataItemType(itemTypeModel, CreatedBy);
                    int x = cnn.Insert(val, "DM_LoaiMatHang");
                    if (x <= 0)
                    {
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_EXCEPTION));
                    }
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }
            return await Task.FromResult(new ReturnSqlModel());
        }

        public async Task<ReturnSqlModel> DeleteItemType(int id, int userId)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    SqlConditions conds = new SqlConditions();
                    conds.Add("IdLMH", id);

                    int result = cnn.Delete(conds, "DM_LoaiMatHang");

                    if (result <= 0)
                    {
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                    }

                    return await Task.FromResult(new ReturnSqlModel()); // Thành công
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }
        }

        public Task<ReturnSqlModel> CreateItem(ItemModel itemModel, int id)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var val = InitDataItem(itemModel, id);
                    int x = cnn.Insert(val, "DM_MatHang");
                    if (x <= 0)
                    {
                        return Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_EXCEPTION));
                    }
                }
                catch (Exception ex)
                {
                    return Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }
            return Task.FromResult(new ReturnSqlModel());
        }

        public async Task<ReturnSqlModel> UpdateItem(ItemModel itemModel, long ModifiedBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("IdMH", itemModel.IdMH);
                    val = InitDataItem(itemModel, ModifiedBy, true);

                    int x = cnn.Update(val, conds, "DM_MatHang");

                    if (x <= 0)
                    {
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                    }

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

        public async Task<ReturnSqlModel> DeleteItem(int id, int userId)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    SqlConditions conds = new SqlConditions();
                    conds.Add("IdMH", id);

                    int result = cnn.Delete(conds, "DM_MatHang");

                    if (result <= 0)
                    {
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                    }

                    return await Task.FromResult(new ReturnSqlModel()); // Thành công
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }
        }

        //DVT
        public async Task<IEnumerable<DVTDTO>> GetDVTList(SqlConditions conds, string? orderByStr = null)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                DataTable dt = new DataTable();
                string sql;

                if (string.IsNullOrWhiteSpace(orderByStr))
                    orderByStr = "TenDVT ASC";

                if (conds != null && conds.Count > 0)
                {
                    sql = $@"
                SELECT IdDVT, TenDVT
                FROM DM_DVT
                WHERE 1=1 AND (where)
                ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql, "(where)", conds);
                }
                else
                {
                    sql = $@"
                SELECT IdDVT, TenDVT
                FROM DM_DVT
                ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql);
                }

                var result = dt.AsEnumerable().Select(row => new DVTDTO
                {
                    IdDVT = Convert.ToInt64(row["IdDVT"]),
                    TenDVT = row["TenDVT"]?.ToString()
                });

                return await Task.FromResult(result);
            }
        }
        private Hashtable InitDataDVT(DVTModel item, long modifiedBy, bool isUpdate = false)
        {
            Hashtable val = new Hashtable();

            void AddIfValid(string key, object? value)
            {
                if (value != null && value != DBNull.Value)
                {
                    val.Add(key, value);
                }
            }

            AddIfValid("TenDVT", item.TenDVT);
            AddIfValid("IdCustomer", item.IdCustomer);
            AddIfValid("isDel", item.IsDel.HasValue ? (item.IsDel.Value ? 1 : 0) : (object?)null);

            if (!isUpdate)
            {
                AddIfValid("CreatedBy", modifiedBy);
                AddIfValid("CreatedDate", DateTime.UtcNow);
            }
            else
            {
                AddIfValid("ModifiedBy", modifiedBy);
                AddIfValid("ModifiedDate", DateTime.UtcNow);
            }

            return val;
        }

        public async Task<ReturnSqlModel> CreateDVT(DVTModel itemModel, int id)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var val = InitDataDVT(itemModel, id);
                    int x = cnn.Insert(val, "DM_DVT");
                    if (x <= 0)
                    {
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_EXCEPTION));
                    }
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }

            return await Task.FromResult(new ReturnSqlModel());
        }

        
        public async Task<ReturnSqlModel> UpdateDVT(DVTModel itemModel, long ModifiedBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("IdDVT", itemModel.IdDVT);
                    val = InitDataDVT(itemModel, ModifiedBy, true);

                    int x = cnn.Update(val, conds, "DM_DVT");

                    if (x <= 0)
                    {
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                    }

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

        public async Task<ReturnSqlModel> DeleteDVT(int id, int userId)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    SqlConditions conds = new SqlConditions();
                    conds.Add("IdDVT", id);

                    int result = cnn.Delete(conds, "DM_DVT");

                    if (result <= 0)
                    {
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                    }

                    return await Task.FromResult(new ReturnSqlModel()); // Thành công
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }
        }

        //Brand
        public async Task<IEnumerable<BrandDTO>> GetBrandList(SqlConditions conds, string? orderByStr = null)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                DataTable dt = new DataTable();
                string sql;

                if (string.IsNullOrWhiteSpace(orderByStr))
                    orderByStr = "TenNhanHieu ASC";

                if (conds != null && conds.Count > 0)
                {
                    sql = $@"
                SELECT *
                FROM DM_NhanHieu
                WHERE 1=1 AND (where)
                ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql, "(where)", conds);
                }
                else
                {
                    sql = $@"
                SELECT *
                FROM DM_NhanHieu
                ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql);
                }

                var result = dt.AsEnumerable().Select(row => new BrandDTO
                {
                    IdNhanHieu = Convert.ToInt32(row["IdNhanHieu"]),
                    TenNhanHieu = row["TenNhanHieu"]?.ToString(),
                    CreatedBy = row["CreatedBy"] != DBNull.Value ? (int?)Convert.ToInt32(row["CreatedBy"]) : null,
                    DeletedBy = row["DeletedBy"] != DBNull.Value ? (int?)Convert.ToInt32(row["DeletedBy"]) : null,
                    ModifiedBy = row["ModifiedBy"] != DBNull.Value ? (int?)Convert.ToInt32(row["ModifiedBy"]) : null,
                    CreatedDate = row["CreatedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["CreatedDate"]) : null,
                    ModifiedDate = row["ModifiedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["ModifiedDate"]) : null,
                    DeletedDate = row["DeletedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["DeletedDate"]) : null,
                    IsDel = row["IsDel"] != DBNull.Value && Convert.ToBoolean(row["IsDel"]),
                    IdCustomer = row["IdCustomer"] != DBNull.Value ? (int?)Convert.ToInt32(row["IdCustomer"]) : null
                });

                return await Task.FromResult(result);
            }
        }

        private Hashtable InitDataBrand(BrandModel brand, long modifiedBy, bool isUpdate = false)
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

            AddIfValid("TenNhanHieu", brand.TenNhanHieu);
            AddIfValid("IdCustomer", brand.IdCustomer);
            AddIfValid("IsDel", brand.IsDel.HasValue ? (brand.IsDel.Value ? 1 : 0) : (object?)null);

            if (!isUpdate)
            {
                AddIfValid("CreatedBy", modifiedBy);
                AddIfValid("CreatedDate", DateTime.UtcNow);
            }
            else
            {
                AddIfValid("ModifiedBy", modifiedBy);
                AddIfValid("ModifiedDate", DateTime.UtcNow);
            }

            return val;
        }

        public async Task<ReturnSqlModel> CreateBrand(BrandModel brandModel, int id)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var val = InitDataBrand(brandModel, id);
                    int x = cnn.Insert(val, "DM_NhanHieu");
                    if (x <= 0)
                    {
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_EXCEPTION));
                    }
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }
            return await Task.FromResult(new ReturnSqlModel());
        }

        public async Task<ReturnSqlModel> UpdateBrand(BrandModel itemModel, long ModifiedBy)
        {
           
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("IdNhanHieu", itemModel.IdNhanHieu);
                    val = InitDataBrand(itemModel, ModifiedBy, true);

                    int x = cnn.Update(val, conds, "DM_NhanHieu");

                    if (x <= 0)
                    {
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                    }

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

        public async Task<ReturnSqlModel> DeleteBrand(int id, int userId)
        {
            using(DpsConnection cnn = new DpsConnection(_connectionString))
            {
                SqlConditions conds = new SqlConditions();
                conds.Add("IdNhanHieu", id, SqlOperator.Equals);
                var result = cnn.Delete(conds, "Dm_NhanHieu");

                if (result <= 0)
                {
                    return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                }

                return await Task.FromResult(new ReturnSqlModel());

            }
        }

        //Origin
        private Hashtable InitDataOrigin(OriginModel origin, long modifiedBy, bool isUpdate = false)
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

            AddIfValid("TenXuatXu", origin.TenXuatXu);
            AddIfValid("IdCustomer", origin.IdCustomer);
            AddIfValid("IsDel", origin.IsDel.HasValue ? (origin.IsDel.Value ? 1 : 0) : (object?)null);

            if (!isUpdate)
            {
                AddIfValid("CreatedBy", modifiedBy);
                AddIfValid("CreatedDate", DateTime.UtcNow);
            }
            else
            {
                AddIfValid("ModifiedBy", modifiedBy);
                AddIfValid("ModifiedDate", DateTime.UtcNow);
            }

            return val;
        }
        public async Task<IEnumerable<OriginModel>> GetOriginList(SqlConditions conds, string? orderByStr = null)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                DataTable dt = new DataTable();
                string sql;

                if (string.IsNullOrWhiteSpace(orderByStr))
                    orderByStr = "TenXuatXu ASC";

                if (conds != null && conds.Count > 0)
                {
                    sql = $@"
                SELECT *
                FROM DM_XuatXu
                WHERE 1=1 AND (where)
                ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql, "(where)", conds);
                }
                else
                {
                    sql = $@"
                SELECT *
                FROM DM_XuatXu
                ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql);
                }

                var result = dt.AsEnumerable().Select(row => new OriginModel
                {
                    IdXuatXu = Convert.ToInt32(row["IdXuatXu"]),
                    TenXuatXu = row["TenXuatXu"]?.ToString(),
                    CreatedBy = row["CreatedBy"] != DBNull.Value ? (int?)Convert.ToInt32(row["CreatedBy"]) : null,
                    DeletedBy = row["DeletedBy"] != DBNull.Value ? (int?)Convert.ToInt32(row["DeletedBy"]) : null,
                    ModifiedBy = row["ModifiedBy"] != DBNull.Value ? (int?)Convert.ToInt32(row["ModifiedBy"]) : null,
                    CreatedDate = row["CreatedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["CreatedDate"]) : null,
                    ModifiedDate = row["ModifiedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["ModifiedDate"]) : null,
                    DeletedDate = row["DeletedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["DeletedDate"]) : null,
                    IsDel = row["IsDel"] != DBNull.Value && Convert.ToBoolean(row["IsDel"]),
                    IdCustomer = row["IdCustomer"] != DBNull.Value ? (int?)Convert.ToInt32(row["IdCustomer"]) : null
                });

                return await Task.FromResult(result);
            }
        }

        public async Task<ReturnSqlModel> CreateOrigin(OriginModel originModel, int id)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var val = InitDataOrigin(originModel, id);
                    int x = cnn.Insert(val, "DM_XuatXu");
                    if (x <= 0)
                    {
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_EXCEPTION));
                    }
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }
            return await Task.FromResult(new ReturnSqlModel());
        }

        public async Task<ReturnSqlModel> UpdateOrigin(OriginModel itemModel, long ModifiedBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("IdXuatXu", itemModel.IdXuatXu);
                    val = InitDataOrigin(itemModel, ModifiedBy, true);

                    int x = cnn.Update(val, conds, "DM_XuatXu");

                    if (x <= 0)
                    {
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                    }
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

        public async Task<ReturnSqlModel> DeleteOrigin(int id, int userId)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    SqlConditions conds = new SqlConditions();
                    conds.Add("IdXuatXu", id);

                    int result = cnn.Delete(conds, "DM_XuatXu");

                    if (result <= 0)
                    {
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                    }

                    return await Task.FromResult(new ReturnSqlModel()); // Thành công
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }
        }

        //Insurance Partner
        private Hashtable InitDataDoitacBaohiem(InsurancePartnerModel model, long modifiedBy, bool isUpdate = false)
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

            AddIfValid("TenDonVi", model.TenDonVi);
            AddIfValid("DiaChi", model.DiaChi);
            AddIfValid("SoDT", model.SoDT);
            AddIfValid("NguoiLienHe", model.NguoiLienHe);
            AddIfValid("GhiChu", model.GhiChu);
            AddIfValid("IsDisable", model.IsDisable.HasValue ? (model.IsDisable.Value ? 1 : 0) : (object?)null);

            if (!isUpdate)
            {
                AddIfValid("NguoiTao", model.NguoiTao);
                AddIfValid("NgayTao", DateTime.UtcNow);
            }
            else
            {
                AddIfValid("NguoiSua", modifiedBy);
                AddIfValid("NgaySua", DateTime.UtcNow);
            }

            return val;
        }
        public async Task<IEnumerable<InsurancePartnerDTO>> GetDoitacBaohiemList(SqlConditions conds, string? orderByStr = null)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                DataTable dt;
                string sql;

                if (string.IsNullOrWhiteSpace(orderByStr))
                    orderByStr = "TenDonVi ASC";

                if (conds != null && conds.Count > 0)
                {
                    sql = $@"
            SELECT * 
            FROM DM_DoiTacBaoHiem 
            WHERE 1=1 AND (where)
            ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql, "(where)", conds);
                }
                else
                {
                    sql = $@"
            SELECT * 
            FROM DM_DoiTacBaoHiem 
            ORDER BY {orderByStr}";

                    dt = cnn.CreateDataTable(sql);
                }

                var result = dt.AsEnumerable().Select(row => new InsurancePartnerDTO
                {
                    Id_DV = Convert.ToInt32(row["Id_DV"]),
                    TenDonVi = row["TenDonVi"]?.ToString(),
                    DiaChi = row["DiaChi"]?.ToString(),
                    SoDT = row["SoDT"]?.ToString(),
                    NguoiLienHe = row["NguoiLienHe"]?.ToString(),
                    GhiChu = row["GhiChu"]?.ToString()
                });

                return await Task.FromResult(result);
            }
        }

        public async Task<ReturnSqlModel> CreateDoitacBaohiem(InsurancePartnerModel itemModel, int id)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var val = InitDataDoitacBaohiem(itemModel, id);
                    int x = cnn.Insert(val, "DM_DoiTacBaoHiem");

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

        public async Task<ReturnSqlModel> UpdateDoitacBaohiem(InsurancePartnerModel itemModel, long ModifiedBy)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var conds = new SqlConditions();
                    conds.Add("Id_DV", itemModel.Id_DV);

                    var val = InitDataDoitacBaohiem(itemModel, ModifiedBy, true);

                    int x = cnn.Update(val, conds, "DM_DoiTacBaoHiem");

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

        public async Task<ReturnSqlModel> DeleteDoitacBaohiem(int id, int userId)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var conds = new SqlConditions();
                    conds.Add("Id_DV", id, SqlOperator.Equals);

                    int result = cnn.Delete(conds, "DM_DoiTacBaoHiem");

                    if (result <= 0)
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }

            return await Task.FromResult(new ReturnSqlModel());
        }

       
    }
}
