using AngularBackEnd.Models.PropertyManagement;
using AngularBackEnd.Services.PropertyMangement;
using DpsLibs.Data;
using JeeBeginner.Classes;
using JeeBeginner.Models.Common;
using JeeBeginner.Services;
using JeeBeginner.Services.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using static JeeBeginner.Models.Common.Paginator;

namespace AngularBackEnd.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/PropertyManagement")]
    [ApiController]
    public class PropertyController : Controller
    {
        public IPropertyManagementService _service;
        public ICustomAuthorizationService _authService;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _jwtSecret;

        public PropertyController(IPropertyManagementService service, ICustomAuthorizationService authService, IConfiguration configuration)
        {
            _service = service;
            _authService = authService;
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _jwtSecret = configuration.GetValue<string>("JWT:Secret");
        }

        //Property Type
        [HttpPost("getAllPropertyType")]
        public async Task<object> GetListPropertyType([FromBody] QueryRequestParams query)
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
                    conds.Add("TenLoai", $"%{query.SearchValue}%", SqlOperator.Like);
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

                var ItemTypeList = await _service.GetPropertyTypeList(conds);
                bool Visible = _authService.IsReadOnlyPermit("107", user.Username);
                bool HasPermission = _authService.HasVisiblePermit("107", user.Username);

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

                return JsonResultCommon.ThanhCong(pagedData, pageModel , Visible, HasPermission);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpPost("updatePropertyType")]
        public async Task<object> UpdatePropertyType(PropertyType propertyTypeModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdLoaiTS from TS_DM_LoaiTS where IdLoaiTS = {propertyTypeModel.IdLoaiTS}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("Loại tài sản");

                var update = await _service.UpdatePropertyType(propertyTypeModel, user.Id);
                if (!update.Susscess)
                {
                    return JsonResultCommon.ThatBai(update.ErrorMessgage);
                }
                return JsonResultCommon.ThanhCong(propertyTypeModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpPost("addPropertyType")]
        public async Task<object> CreatePropertyType(PropertyType propertyTypeModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");


                //string sqlCheckUsername = $"select Username from AccountList where Username = '{accountModel.Username}'";
                //bool isExistUsername = GeneralService.IsExistDB(sqlCheckUsername, _connectionString);
                //if (isExistUsername) return JsonResultCommon.Trung("Username");

                var create = await _service.CreatePropertyType(propertyTypeModel, user.Id);
                if (!create.Susscess)
                {
                    return JsonResultCommon.ThatBai(create.ErrorMessgage);
                }

                return JsonResultCommon.ThanhCong(propertyTypeModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpDelete("PropertyType/{id}")]
        public async Task<object> DeletePropertyType(int id)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheck = $"SELECT IdLoaiTS FROM TS_DM_LoaiTS WHERE IdLoaiTS = {id}";
                bool exists = GeneralService.IsExistDB(sqlCheck, _connectionString);
                if (!exists) return JsonResultCommon.KhongTonTai("Loại mặt hàng");

                var result = await _service.DeletePropertyType(id, user.Id);
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

        //Group Property Type
        [HttpPost("getAllGroupPropertyType")]
        public async Task<object> GetListGroupPropertyType([FromBody] QueryRequestParams query)
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
                    conds.Add("TenLoai", $"%{query.SearchValue}%", SqlOperator.Like);

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

                var ItemTypeList = await _service.GetGroupPropertyTypeList(conds);
                bool Visible = _authService.IsReadOnlyPermit("108", user.Username);
                bool HasPermission = _authService.HasVisiblePermit("108", user.Username);

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

        [HttpPost("updateGroupPropertyType")]
        public async Task<object> UpdateGroupPropertyType(GroupPropertyType groupPropertyTypeModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdPNTS from TS_DM_PhanNhomTS where IdPNTS = {groupPropertyTypeModel.IdPNTS}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("Nhom Loại tài sản");

                var update = await _service.UpdateGroupPropertyType(groupPropertyTypeModel, user.Id);
                if (!update.Susscess)
                {
                    return JsonResultCommon.ThatBai(update.ErrorMessgage);
                }
                return JsonResultCommon.ThanhCong(groupPropertyTypeModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpPost("addGroupPropertyType")]
        public async Task<object> CreateGroupPropertyType(GroupPropertyType groupPropertyTypeModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");


                //string sqlCheckUsername = $"select Username from AccountList where Username = '{accountModel.Username}'";
                //bool isExistUsername = GeneralService.IsExistDB(sqlCheckUsername, _connectionString);
                //if (isExistUsername) return JsonResultCommon.Trung("Username");

                var create = await _service.CreateGroupPropertyType(groupPropertyTypeModel, user.Id);
                if (!create.Susscess)
                {
                    return JsonResultCommon.ThatBai(create.ErrorMessgage);
                }

                return JsonResultCommon.ThanhCong(groupPropertyTypeModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpDelete("GroupPropertyType/{id}")]
        public async Task<object> DeleteGroupPropertyType(int id)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheck = $"SELECT IdPNTS FROM TS_DM_PhanNhomTS WHERE IdPNTS = {id}";
                bool exists = GeneralService.IsExistDB(sqlCheck, _connectionString);
                if (!exists) return JsonResultCommon.KhongTonTai("Loại mặt hàng");

                var result = await _service.DeleteGroupPropertyType(id, user.Id);
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

        //Ly do tang giam tai san
        [HttpPost("getAllReason")]
        public async Task<object> GetListReason([FromBody] QueryRequestParams query)
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
                    conds.Add("TenTangGiam", $"%{query.SearchValue}%", SqlOperator.Like);

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

                var ItemTypeList = await _service.GetReasonList(conds);
                bool Visible = _authService.IsReadOnlyPermit("109", user.Username);
                bool HasPermission = _authService.HasVisiblePermit("109", user.Username);

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

        [HttpPost("updateReason")]
        public async Task<object> UpdateReason(Reason reasonModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdRow from TS_DM_LyDoTangGiamTS where IdRow = {reasonModel.IdRow}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("");

                var update = await _service.UpdateReason(reasonModel, user.Id);
                if (!update.Susscess)
                {
                    return JsonResultCommon.ThatBai(update.ErrorMessgage);
                }
                return JsonResultCommon.ThanhCong(reasonModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpPost("addReason")]
        public async Task<object> CreateReason(Reason reasonModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");


                //string sqlCheckUsername = $"select Username from AccountList where Username = '{accountModel.Username}'";
                //bool isExistUsername = GeneralService.IsExistDB(sqlCheckUsername, _connectionString);
                //if (isExistUsername) return JsonResultCommon.Trung("Username");

                var create = await _service.CreateReason(reasonModel, user.Id);
                if (!create.Susscess)
                {
                    return JsonResultCommon.ThatBai(create.ErrorMessgage);
                }

                return JsonResultCommon.ThanhCong(reasonModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpDelete("Reason/{id}")]
        public async Task<object> DeleteReason(int id)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheck = $"SELECT IdRow FROM TS_DM_LyDoTangGiamTS WHERE IdRow = {id}";
                bool exists = GeneralService.IsExistDB(sqlCheck, _connectionString);
                if (!exists) return JsonResultCommon.KhongTonTai("Lý do tăng giảm tài sản");

                var result = await _service.DeleteReason(id, user.Id);
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
