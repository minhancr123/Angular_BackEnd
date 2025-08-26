using AngularBackEnd.MiddleWare;
using AngularBackEnd.Models.InventoryManagement;
using AngularBackEnd.Services.InventoryMangement;
using DpsLibs.Data;
using JeeBeginner.Classes;
using JeeBeginner.Models.AccountManagement;
using JeeBeginner.Models.Common;
using JeeBeginner.Services;
using JeeBeginner.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using static JeeBeginner.Models.Common.Paginator;

namespace AngularBackEnd.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/InventoryManagement")]
    [ApiController]
    public class InventoryController : Controller
    {
        public IIventoryManagementService _service;
        public ICustomAuthorizationService _authService;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _jwtSecret;

        public InventoryController(IIventoryManagementService service , ICustomAuthorizationService authService, IConfiguration configuration)
        {
            _authService = authService;
              _service = service;
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _jwtSecret = configuration.GetValue<string>("JWT:Secret");
        }
        #region Itemtype

        [HttpPost("GetAll")]
        public async Task<object> GetListItemType([FromBody] QueryRequestParams query)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);

                if (user == null)
                    JsonResultCommon.BatBuoc("Đăng nhập");

                query ??= new QueryRequestParams();
                query.Paginator = new Paginator(1, 10, 0);

                BaseModel<object> model = new BaseModel<object>();
                PageModel pageModel = new PageModel();
                SqlConditions conds = new SqlConditions();

                // Tìm kiếm theo tên
                if (!string.IsNullOrEmpty(query.SearchValue))
                {
                    conds.Add("TenLMH", $"%{query.SearchValue}%", SqlOperator.Like);

                }

                if(query.Filter != null)
                {
                    foreach(var item in query.Filter)
                    {
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            switch (item.Key.ToLower())
                            {
                                case "tenloai":
                                    conds.Add("TenLMH", $"{item.Value}", SqlOperator.Like);
                                    break;
                            }
                        }
                    }
                }

                var ItemTypeList = await _service.GetItemTypeList(conds);
                bool Visible = _authService.IsReadOnlyPermit("101", user.Username);
                bool HasPermission = _authService.HasVisiblePermit("101", user.Username);
                if (ItemTypeList == null)
                    JsonResultCommon.KhongTonTai();

                if(!ItemTypeList.Any())
                    JsonResultCommon.ThatBai("Không có dữ liệu");

                // Phân trang
                int total = ItemTypeList.Count();
                pageModel.TotalCount = total;
                pageModel.AllPage = (int)Math.Ceiling(total / (decimal)query.Paginator.PageSize);
                pageModel.Size = query.Paginator.PageSize;
                pageModel.Page = query.Paginator.PageIndex;

                var pagedData = ItemTypeList.Skip((query.Paginator.PageIndex - 1) * query.Paginator.PageSize).Take(query.Paginator.PageSize);

                return JsonResultCommon.ThanhCong(pagedData, pageModel, Visible , HasPermission);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [CheckPermission("101")]
        [HttpPost("updateItemType")]
        public async Task<object> UpdateAccount(ItemTypeModel itemTypeModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdLMH from DM_LoaiMatHang where IdLMH = {itemTypeModel.IdLMH}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("Itemtype");

                var update = await _service.UpdateItemType(itemTypeModel, user.Id);
                if (!update.Susscess)
                {
                    return JsonResultCommon.ThatBai(update.ErrorMessgage);
                }
                return JsonResultCommon.ThanhCong(itemTypeModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [CheckPermission("101")]
        [HttpPost("addItemType")]
        public async Task<object> CreateItemType(ItemTypeModel itemTypeModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");


                //string sqlCheckUsername = $"select Username from AccountList where Username = '{accountModel.Username}'";
                //bool isExistUsername = GeneralService.IsExistDB(sqlCheckUsername, _connectionString);
                //if (isExistUsername) return JsonResultCommon.Trung("Username");

                var create = await _service.CreateItemType(itemTypeModel, user.Id);
                if (!create.Susscess)
                {
                    return JsonResultCommon.ThatBai(create.ErrorMessgage);
                }

                return JsonResultCommon.ThanhCong(itemTypeModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [CheckPermission("101")]
        [HttpDelete("{id}")]
        public async Task<object> DeleteItemType(int id)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheck = $"SELECT IdLMH FROM DM_LoaiMatHang WHERE IdLMH = {id}";
                bool exists = GeneralService.IsExistDB(sqlCheck, _connectionString);
                if (!exists) return JsonResultCommon.KhongTonTai("Loại mặt hàng");

                var result = await _service.DeleteItemType(id, user.Id);
                if (!result.Susscess)
                {
                    return JsonResultCommon.ThatBai(result.ErrorMessgage);
                }

                return JsonResultCommon.ThanhCong("Xoá thành công");
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion



        //Item
        #region Item
        [HttpPost("GetAllItem")]
        public async Task<object> GetListItem([FromBody] QueryRequestParams query)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);

                if (user == null)
                    JsonResultCommon.BatBuoc("Đăng nhập");

                query ??= new QueryRequestParams();
                query.Paginator = new Paginator(1, 10, 0);

                BaseModel<object> model = new BaseModel<object>();
                PageModel pageModel = new PageModel();
                SqlConditions conds = new SqlConditions();

                // Tìm kiếm theo tên
                if (!string.IsNullOrEmpty(query.SearchValue))
                {
                    conds.Add("TenMatHang", $"%{query.SearchValue}%", SqlOperator.Like);

                }

                if (query.Filter != null)
                {
                    foreach (var item in query.Filter)
                    {
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            switch (item.Key.ToLower())
                            {
                                case "iddvt":
                                    conds.Add("IdDVT", item.Value, SqlOperator.Equals);
                                    break;
                                case "idlmh":
                                    conds.Add("IdLMH", item.Value, SqlOperator.Equals);
                                    break;
                            }
                        }
                    }
                }


                var ItemList = await _service.GetItemList(conds);
                bool Visible = _authService.IsReadOnlyPermit("102", user.Username);

                bool HasPermission = _authService.HasVisiblePermit("102", user.Username);
                if (ItemList == null)
                    JsonResultCommon.KhongTonTai();

                if (!ItemList.Any())
                    JsonResultCommon.ThatBai("Không có dữ liệu");

                // Phân trang
                int total = ItemList.Count();
                pageModel.TotalCount = total;
                pageModel.AllPage = (int)Math.Ceiling(total / (decimal)query.Paginator.PageSize);
                pageModel.Size = query.Paginator.PageSize;
                pageModel.Page = query.Paginator.PageIndex;

                var pagedData = ItemList.Skip((query.Paginator.PageIndex - 1) * query.Paginator.PageSize).Take(query.Paginator.PageSize);

                return JsonResultCommon.ThanhCong(pagedData, pageModel, Visible , HasPermission);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpGet("GetItemById")]
        public async Task<object> GetItemById([FromQuery] string id)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);

                if (user == null)
                    return JsonResultCommon.BatBuoc("Đăng nhập");

                if (string.IsNullOrEmpty(id))
                    return JsonResultCommon.KhongTonTai("Id mặt hàng không hợp lệ");

                var item = await _service.GetItemById(id);

                if (item == null)
                    return JsonResultCommon.KhongTonTai("Không tìm thấy mặt hàng");

                return JsonResultCommon.ThanhCong(item.First());
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [CheckPermission("102")]
        [HttpPost("addItem")]
        public async Task<object> CreateItem([FromBody] ItemModel itemModel)
        {
            try
            {
                if (itemModel == null)
                {
                    using (StreamReader reader = new StreamReader(Request.Body))
                    {
                        var rawBody = await reader.ReadToEndAsync();
                        Console.WriteLine("🔥 Raw body nhận được: " + rawBody);
                    }

                    return JsonResultCommon.ThatBai("❌ itemModel is null");
                }
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");


                //string sqlCheckUsername = $"select Username from AccountList where Username = '{accountModel.Username}'";
                //bool isExistUsername = GeneralService.IsExistDB(sqlCheckUsername, _connectionString);
                //if (isExistUsername) return JsonResultCommon.Trung("Username");

                var create = await _service.CreateItem(itemModel, user.Id);
                if (!create.Susscess)
                {
                    return JsonResultCommon.ThatBai(create.ErrorMessgage);
                }

                return JsonResultCommon.ThanhCong(itemModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpPost("importItems")]
        public async Task<object> ImportItems([FromBody] List<ItemModel> items)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                foreach (var item in items)
                {
                    var create = await _service.CreateItem(item, user.Id);
                    if (!create.Susscess)
                        return JsonResultCommon.ThatBai($"Lỗi với mặt hàng {item.TenMatHang}: {create.ErrorMessgage}");
                }

                return JsonResultCommon.ThanhCong("Import thành công");
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }   
        }

        [CheckPermission("102")]
        [HttpPost("uploadFile")]
        public async Task<object> UploadFile(IFormFile file)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                if (file == null || file.Length == 0)
                {
                    return JsonResultCommon.ThatBai("File không hợp lệ");
                }

                // Đường dẫn thư mục để lưu file (ví dụ: wwwroot/uploads)
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Tạo tên file duy nhất
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);

                // Lưu file vào server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Tạo URL để client có thể truy cập
                var request = HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";
                var fileUrl = $"{baseUrl}/uploads/{fileName}";

                return JsonResultCommon.ThanhCong(fileUrl);

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        [CheckPermission("102")]
        [HttpPost("updateItem")]
        public async Task<object> UpdateItem([FromBody] ItemModel itemModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdMH from DM_MatHang where IdMH = {itemModel.IdMH}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("mặt hàng");

                var update = await _service.UpdateItem(itemModel, user.Id);
                if (!update.Susscess)
                {
                    return JsonResultCommon.ThatBai(update.ErrorMessgage);
                }
                return JsonResultCommon.ThanhCong(itemModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [CheckPermission("102")]
        [HttpDelete("Item/{id}")]
        public async Task<object> DeleteItem(int id)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheck = $"SELECT IdMH FROM DM_MatHang WHERE IdMH = {id}";
                bool exists = GeneralService.IsExistDB(sqlCheck, _connectionString);
                if (!exists) return JsonResultCommon.KhongTonTai("Mặt hàng");

                var result = await _service.DeleteItem(id, user.Id);
                if (!result.Susscess)
                {
                    return JsonResultCommon.ThatBai(result.ErrorMessgage);
                }

                return JsonResultCommon.ThanhCong("Xoá thành công");
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        //DVT
        [HttpPost("GetAllDVT")]
        public async Task<object> GetDVTList([FromBody] QueryRequestParams query)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);

                if (user == null)
                    JsonResultCommon.BatBuoc("Đăng nhập");

                query ??= new QueryRequestParams();
                query.Paginator = new Paginator(1, 10, 0);

                BaseModel<object> model = new BaseModel<object>();
                PageModel pageModel = new PageModel();
                SqlConditions conds = new SqlConditions();

                // Tìm kiếm theo tên
                if (!string.IsNullOrEmpty(query.SearchValue))
                {
                    conds.Add("TenDVT", $"%{query.SearchValue}%", SqlOperator.Like);

                }

                //if (query.Filter != null)
                //{
                //    foreach (var item in query.Filter)
                //    {
                //        if (!string.IsNullOrEmpty(item.Value))
                //        {
                //            switch (item.Key.ToLower())
                //            {
                //                case "tenloai":
                //                    conds.Add("TenLMH", $"{item.Value}", SqlOperator.Like);
                //                    break;
                //            }
                //        }
                //    }
                //}

                var ItemTypeList = await _service.GetDVTList(conds);
                bool Visible = _authService.IsReadOnlyPermit("103", user.Username);
                bool HasPermission = _authService.HasVisiblePermit("103", user.Username);
                if (ItemTypeList == null)
                    JsonResultCommon.KhongTonTai();

                if (!ItemTypeList.Any())
                    JsonResultCommon.ThatBai("Không có dữ liệu");

                // Phân trang
                int total = ItemTypeList.Count();
                if (query.Paginator.PageSize <= 0) query.Paginator.PageSize = 10;
                if (query.Paginator.PageIndex <= 0) query.Paginator.PageIndex = 1;

                int allPage = (int)Math.Ceiling(total / (decimal)query.Paginator.PageSize);
                if (query.Paginator.PageIndex > allPage && allPage > 0)
                    query.Paginator.PageIndex = allPage;

                pageModel.TotalCount = total;
                pageModel.AllPage = allPage;
                pageModel.Size = query.Paginator.PageSize;
                pageModel.Page = query.Paginator.PageIndex;

                var pagedData = ItemTypeList
                    .Skip((query.Paginator.PageIndex - 1) * query.Paginator.PageSize)
                    .Take(query.Paginator.PageSize);

                return JsonResultCommon.ThanhCong(pagedData, pageModel, Visible , HasPermission);

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [CheckPermission("103")]
        [HttpPost("addDVT")]
        public async Task<object> CreateDVT(DVTModel itemModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");


                //string sqlCheckUsername = $"select Username from AccountList where Username = '{accountModel.Username}'";
                //bool isExistUsername = GeneralService.IsExistDB(sqlCheckUsername, _connectionString);
                //if (isExistUsername) return JsonResultCommon.Trung("Username");

                var create = await _service.CreateDVT(itemModel, user.Id);
                if (!create.Susscess)
                {
                    return JsonResultCommon.ThatBai(create.ErrorMessgage);
                }

                return JsonResultCommon.ThanhCong(itemModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        


        [CheckPermission("103")]
        [HttpPost("updateDVT")]
        public async Task<object> UpdateDVT(DVTModel itemModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdDVT from DM_DVT where IdDVT = {itemModel.IdDVT}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("Đơn vị tính");

                var update = await _service.UpdateDVT(itemModel, user.Id);
                if (!update.Susscess)
                {
                    return JsonResultCommon.ThatBai(update.ErrorMessgage);
                }
                return JsonResultCommon.ThanhCong(itemModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [CheckPermission("103")]
        [HttpDelete("DVT/{id}")]
        public async Task<object> DeleteDVT(int id)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheck = $"SELECT IdDVT FROM DM_DVT WHERE IdDVT = {id}";
                bool exists = GeneralService.IsExistDB(sqlCheck, _connectionString);
                if (!exists) return JsonResultCommon.KhongTonTai("Đơn vị tính");

                var result = await _service.DeleteDVT(id, user.Id);
                if (!result.Susscess)
                {
                    return JsonResultCommon.ThatBai(result.ErrorMessgage);
                }

                return JsonResultCommon.ThanhCong("Xoá thành công");
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        //Brand
        [HttpPost("GetAllBrand")]
        public async Task<object> GetBrandList([FromBody] QueryRequestParams query)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);

                if (user == null)
                    JsonResultCommon.BatBuoc("Đăng nhập");

                query ??= new QueryRequestParams();
                query.Paginator = new Paginator(1, 10, 0);

                BaseModel<object> model = new BaseModel<object>();
                PageModel pageModel = new PageModel();
                SqlConditions conds = new SqlConditions();

                // Tìm kiếm theo tên
                if (!string.IsNullOrEmpty(query.SearchValue))
                {
                    conds.Add("TenNhanHieu", $"%{query.SearchValue}%", SqlOperator.Like);

                }

                //if (query.Filter != null)
                //{
                //    foreach (var item in query.Filter)
                //    {
                //        if (!string.IsNullOrEmpty(item.Value))
                //        {
                //            switch (item.Key.ToLower())
                //            {
                //                case "tenloai":
                //                    conds.Add("TenLMH", $"{item.Value}", SqlOperator.Like);
                //                    break;
                //            }
                //        }
                //    }
                //}
                string orderByStr = null;
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName))
                {
                    orderByStr = query.Sort.ColumnName + " " +
                        (query.Sort.Direction.Equals("asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc");
                }
                var ItemTypeList = await _service.GetBrandList(conds, orderByStr);
                bool Visible = _authService.IsReadOnlyPermit("104", user.Username);
                bool HasPermission = _authService.HasVisiblePermit("104", user.Username);
                if (ItemTypeList == null)
                    JsonResultCommon.KhongTonTai();

                if (!ItemTypeList.Any())
                    JsonResultCommon.ThatBai("Không có dữ liệu");

                // Phân trang
                int total = ItemTypeList.Count();
                pageModel.TotalCount = total;
                pageModel.AllPage = (int)Math.Ceiling(total / (decimal)query.Paginator.PageSize);
                pageModel.Size = query.Paginator.PageSize;
                pageModel.Page = query.Paginator.PageIndex;

                var pagedData = ItemTypeList.Skip((query.Paginator.PageIndex - 1) * query.Paginator.PageSize).Take(query.Paginator.PageSize);

                return JsonResultCommon.ThanhCong(pagedData, pageModel, Visible , HasPermission);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }


        [HttpPost("addBrand")]
        public async Task<object> CreateBrand([FromBody] BrandModel brandModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");


                //string sqlCheckUsername = $"select Username from AccountList where Username = '{accountModel.Username}'";
                //bool isExistUsername = GeneralService.IsExistDB(sqlCheckUsername, _connectionString);
                //if (isExistUsername) return JsonResultCommon.Trung("Username");

                var create = await _service.CreateBrand(brandModel, user.Id);
                if (!create.Susscess)
                {
                    return JsonResultCommon.ThatBai(create.ErrorMessgage);
                }

                return JsonResultCommon.ThanhCong(brandModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpPost("updateBrand")]
        public async Task<object> UpdateBrand(BrandModel itemModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdNhanHieu from DM_NhanHieu where IdNhanHieu = {itemModel.IdNhanHieu}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("Itemtype");

                var update = await _service.UpdateBrand(itemModel, user.Id);
                if (!update.Susscess)
                {
                    return JsonResultCommon.ThatBai(update.ErrorMessgage);
                }
                return JsonResultCommon.ThanhCong(itemModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpDelete("Brand/{id}")]
        public async Task<object> DeleteBrand(int id)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheck = $"SELECT IdNhanHieu FROM DM_NhanHieu WHERE IdNhanHieu = {id}";
                bool exists = GeneralService.IsExistDB(sqlCheck, _connectionString);
                if (!exists) return JsonResultCommon.KhongTonTai("Nhãn hiệu");

                var result = await _service.DeleteBrand(id, user.Id);
                if (!result.Susscess)
                {
                    return JsonResultCommon.ThatBai(result.ErrorMessgage);
                }

                return JsonResultCommon.ThanhCong("Xoá thành công");
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        //Origin
        [HttpPost("GetAllOrigin")]
        public async Task<object> GetOriginList([FromBody] QueryRequestParams query)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);

                if (user == null)
                    JsonResultCommon.BatBuoc("Đăng nhập");

                query ??= new QueryRequestParams();
                query.Paginator = new Paginator(1, 10, 0);

                BaseModel<object> model = new BaseModel<object>();
                PageModel pageModel = new PageModel();
                SqlConditions conds = new SqlConditions();

                // Tìm kiếm theo tên
                if (!string.IsNullOrEmpty(query.SearchValue))
                {
                    conds.Add("TenXuatXu", $"%{query.SearchValue}%", SqlOperator.Like);

                }

                //if (query.Filter != null)
                //{
                //    foreach (var item in query.Filter)
                //    {
                //        if (!string.IsNullOrEmpty(item.Value))
                //        {
                //            switch (item.Key.ToLower())
                //            {
                //                case "tenloai":
                //                    conds.Add("TenLMH", $"{item.Value}", SqlOperator.Like);
                //                    break;
                //            }
                //        }
                //    }
                //}
                string orderByStr = null;
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName))
                {
                    orderByStr = query.Sort.ColumnName + " " +
                        (query.Sort.Direction.Equals("asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc");
                }
                var ItemTypeList = await _service.GetOriginList(conds, orderByStr);
                bool Visible = _authService.IsReadOnlyPermit("105", user.Username);
                bool HasPermission = _authService.HasVisiblePermit("105", user.Username);

                if (ItemTypeList == null)
                    JsonResultCommon.KhongTonTai();

                if (!ItemTypeList.Any())
                    JsonResultCommon.ThatBai("Không có dữ liệu");

                // Phân trang
                int total = ItemTypeList.Count();
                if (query.Paginator.PageSize <= 0) query.Paginator.PageSize = 10;
                if (query.Paginator.PageIndex <= 0) query.Paginator.PageIndex = 1;

                int allPage = (int)Math.Ceiling(total / (decimal)query.Paginator.PageSize);
                if (query.Paginator.PageIndex > allPage && allPage > 0)
                    query.Paginator.PageIndex = allPage;

                pageModel.TotalCount = total;
                pageModel.AllPage = allPage;
                pageModel.Size = query.Paginator.PageSize;
                pageModel.Page = query.Paginator.PageIndex;

                var pagedData = ItemTypeList
                    .Skip((query.Paginator.PageIndex - 1) * query.Paginator.PageSize)
                    .Take(query.Paginator.PageSize);

                return JsonResultCommon.ThanhCong(pagedData, pageModel, Visible , HasPermission);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }


        [HttpPost("addOrigin")]
        public async Task<object> CreateOrigin([FromBody] OriginModel originModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");


                //string sqlCheckUsername = $"select Username from AccountList where Username = '{accountModel.Username}'";
                //bool isExistUsername = GeneralService.IsExistDB(sqlCheckUsername, _connectionString);
                //if (isExistUsername) return JsonResultCommon.Trung("Username");

                var create = await _service.CreateOrigin(originModel, user.Id);
                if (!create.Susscess)
                {
                    return JsonResultCommon.ThatBai(create.ErrorMessgage);
                }

                return JsonResultCommon.ThanhCong(originModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpPost("updateOrigin")]
        public async Task<object> UpdateOrigin(OriginModel itemModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdXuatXu from DM_XuatXu where IdXuatXu = {itemModel.IdXuatXu}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("Xuất xứ");

                var update = await _service.UpdateOrigin(itemModel, user.Id);
                if (!update.Susscess)
                {
                    return JsonResultCommon.ThatBai(update.ErrorMessgage);
                }
                return JsonResultCommon.ThanhCong(itemModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpDelete("Origin/{id}")]
        public async Task<object> DeleteOrigin(int id)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheck = $"SELECT IdXuatXu FROM DM_XuatXu WHERE IdXuatXu = {id}";
                bool exists = GeneralService.IsExistDB(sqlCheck, _connectionString);
                if (!exists) return JsonResultCommon.KhongTonTai("Xuất xứ");

                var result = await _service.DeleteOrigin(id, user.Id);
                if (!result.Susscess)
                {
                    return JsonResultCommon.ThatBai(result.ErrorMessgage);
                }

                return JsonResultCommon.ThanhCong("Xoá thành công");
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        //Insurance Partner
        [HttpPost("GetAllDoitacBaohiem")]
        public async Task<object> GetDoitacBaohiemList([FromBody] QueryRequestParams query)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);

                if (user == null)
                    JsonResultCommon.BatBuoc("Đăng nhập");

                query ??= new QueryRequestParams();
                query.Paginator = new Paginator(1, 10, 0);

                BaseModel<object> model = new BaseModel<object>();
                PageModel pageModel = new PageModel();
                SqlConditions conds = new SqlConditions();

                // Tìm kiếm theo tên
                if (!string.IsNullOrEmpty(query.SearchValue))
                {
                    conds.Add("TenDonVi", $"%{query.SearchValue}%", SqlOperator.Like);

                }

                //if (query.Filter != null)
                //{
                //    foreach (var item in query.Filter)
                //    {
                //        if (!string.IsNullOrEmpty(item.Value))
                //        {
                //            switch (item.Key.ToLower())
                //            {
                //                case "tenloai":
                //                    conds.Add("TenLMH", $"{item.Value}", SqlOperator.Like);
                //                    break;
                //            }
                //        }
                //    }
                //}
                string orderByStr = null;
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName))
                {
                    orderByStr = query.Sort.ColumnName + " " +
                        (query.Sort.Direction.Equals("asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc");
                }
                var ItemTypeList = await _service.GetDoitacBaohiemList(conds, orderByStr);
                bool Visible = _authService.IsReadOnlyPermit("106", user.Username);
                bool HasPermission = _authService.HasVisiblePermit("106", user.Username);

                if (ItemTypeList == null)
                    JsonResultCommon.KhongTonTai();

                if (!ItemTypeList.Any())
                    JsonResultCommon.ThatBai("Không có dữ liệu");

                // Phân trang
                int total = ItemTypeList.Count();
                pageModel.TotalCount = total;
                pageModel.AllPage = (int)Math.Ceiling(total / (decimal)query.Paginator.PageSize);
                pageModel.Size = query.Paginator.PageSize;
                pageModel.Page = query.Paginator.PageIndex;

                var pagedData = ItemTypeList.Skip((query.Paginator.PageIndex - 1) * query.Paginator.PageSize).Take(query.Paginator.PageSize);

                return JsonResultCommon.ThanhCong(pagedData, pageModel, Visible, HasPermission);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }


        [HttpPost("addDoitacBaohiem")]
        public async Task<object> CreateDoitacBaohiem([FromBody] InsurancePartnerModel itemModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");


                //string sqlCheckUsername = $"select Username from AccountList where Username = '{accountModel.Username}'";
                //bool isExistUsername = GeneralService.IsExistDB(sqlCheckUsername, _connectionString);
                //if (isExistUsername) return JsonResultCommon.Trung("Username");

                var create = await _service.CreateDoitacBaohiem(itemModel, user.Id);
                if (!create.Susscess)
                {
                    return JsonResultCommon.ThatBai(create.ErrorMessgage);
                }

                return JsonResultCommon.ThanhCong(itemModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpPost("updateDoitacBaohiem")]
        public async Task<object> UpdateDoitacBaohiem(InsurancePartnerModel itemModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select Id_DV from DM_DoiTacBaoHiem where Id_DV = {itemModel.Id_DV}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("Đối tác bảo hiểm");

                var update = await _service.UpdateDoitacBaohiem(itemModel, user.Id);
                if (!update.Susscess)
                {
                    return JsonResultCommon.ThatBai(update.ErrorMessgage);
                }
                return JsonResultCommon.ThanhCong(itemModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpDelete("DoitacBaohiem/{id}")]
        public async Task<object> DeleteDoitacBaohiem(int id)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheck = $"SELECT Id_DV FROM DM_DoiTacBaoHiem WHERE Id_DV = {id}";
                bool exists = GeneralService.IsExistDB(sqlCheck, _connectionString);
                if (!exists) return JsonResultCommon.KhongTonTai("Đối tác bảo hiểm");

                var result = await _service.DeleteDoitacBaohiem(id, user.Id);
                if (!result.Susscess)
                {
                    return JsonResultCommon.ThatBai(result.ErrorMessgage);
                }

                return JsonResultCommon.ThanhCong("Xoá thành công");
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
    }
}
