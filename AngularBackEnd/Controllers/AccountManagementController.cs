using AngularBackEnd.Models.AccountManagement;
using DpsLibs.Data;
using JeeBeginner.Classes;
using JeeBeginner.Models.AccountManagement;
using JeeBeginner.Models.Common;
using JeeBeginner.Services;
using JeeBeginner.Services.AccountManagement;
using JeeBeginner.Services.Authorization;
using JeeBeginner.Services.PartnerManagement;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static JeeBeginner.Models.Common.Paginator;

namespace JeeBeginner.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/accountmanagement")]
    [ApiController]
    public class AccountManagementController : ControllerBase
    {
        private readonly IAccountManagementService _service;
        private readonly IPartnerManagementService _partnerService;
        private readonly ICustomAuthorizationService _authService;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _jwtSecret;

        public AccountManagementController(IAccountManagementService accountManagementService, IPartnerManagementService partner, IConfiguration configuration, ICustomAuthorizationService authService)
        {
            _service = accountManagementService;
            _configuration = configuration;
            _authService = authService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _partnerService = partner;
            _jwtSecret = configuration.GetValue<string>("JWT:Secret");
        }

        [HttpPost("Get_DS")]
        public async Task<object> GetListDS([FromBody] QueryRequestParams query)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user == null)
                    return JsonResultCommon.BatBuoc("Đăng nhập");

                query ??= new QueryRequestParams();
                query.Paginator ??= new Paginator(1, 10, 0);


                BaseModel<object> model = new BaseModel<object>();
                PageModel pageModel = new PageModel();
                SqlConditions conds = new SqlConditions();

                string orderByStr = "AccountList.RowID asc";
                conds.Add("PartnerList.IsLock", 0); // Mặc định lọc đối tác chưa khóa

                var partnerObj = GeneralService.GetObjectDB(
                    $"SELECT PartnerList.RowID FROM AccountList JOIN PartnerList ON AccountList.PartnerID = PartnerList.RowID WHERE AccountList.RowID = {user.Id}",
                    _connectionString
                );

                if (partnerObj == null)
                    return JsonResultCommon.KhongTonTai("Không tìm thấy đối tác của tài khoản");

                string partnerID = partnerObj.ToString();

                // Nếu không phải tài khoản master thì chỉ xem dữ liệu của đối tác mình
                if (!user.IsMasterAccount)
                {
                    conds.Add("PartnerList.RowID", partnerID);
                }

                // Mapping cột để sort
                var filter = new Dictionary<string, string>
        {
            { "stt", "AccountList.RowID" },
            { "username", "AccountList.Username" },
            { "tendoitac", "PartnerList.PartnerName" },
            { "tinhtrang", "AccountList.IsLock" },
        };

                // Xử lý sort
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && filter.ContainsKey(query.Sort.ColumnName))
                {
                    orderByStr = filter[query.Sort.ColumnName] + " " +
                        (query.Sort.Direction.Equals("asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc");
                }

                // Xử lý filter
                if (query.Filter != null)
                {
                    if (query.Filter.TryGetValue("status", out var status) && status != "-1")
                    {
                        conds.Add("AccountList.IsLock", status);
                    }

                    if (query.Filter.TryGetValue("doitac", out var doitac) && doitac != "-1")
                    {
                        conds.Add("PartnerList.RowID", doitac);
                    }
                }

                // Tìm kiếm theo tên
                if (!string.IsNullOrEmpty(query.SearchValue))
                {
                    conds.Add("AccountList.Fullname", $"%{query.SearchValue}%" , SqlOperator.Like);
                }


                // Kiểm tra quyền xem
                bool visible = !_authService.IsReadOnlyPermit("1", user.Username);

                var customerList = await _service.GetAll(conds, orderByStr);

                if (customerList == null)
                    return JsonResultCommon.KhongTonTai();

                if (!customerList.Any())
                    return JsonResultCommon.ThatBai("Không có dữ liệu");

                // Phân trang
                int total = customerList.Count();
                pageModel.TotalCount = total;
                pageModel.AllPage = (int)Math.Ceiling(total / (decimal)query.Paginator.PageSize);
                pageModel.Size = query.Paginator.PageSize;
                pageModel.Page = query.Paginator.PageIndex;

                var pagedData = customerList
                    .Skip((query.Paginator.PageIndex - 1) * query.Paginator.PageSize)
                    .Take(query.Paginator.PageSize);

                return JsonResultCommon.ThanhCong(pagedData, pageModel, visible);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        [HttpGet("Get_User/{id}")]
        public async Task<object> GeUserById(int id)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user == null)
                    return JsonResultCommon.BatBuoc("Đăng nhập");

                // Lấy PartnerID của user cần xem
                var partnerObj = GeneralService.GetObjectDB(
                    $"SELECT PartnerList.RowID FROM AccountList " +
                    $"JOIN PartnerList ON AccountList.PartnerID = PartnerList.RowID " +
                    $"WHERE AccountList.RowID = {id}",
                    _connectionString
                );

                if (partnerObj == null)
                    return JsonResultCommon.KhongTonTai("Không tìm thấy đối tác của tài khoản");

                string partnerID = partnerObj.ToString();

                // Nếu không phải tài khoản master thì chỉ xem dữ liệu của đối tác mình
                //if (!user.IsMasterAccount && user.PartnerID.ToString() != partnerID)
                //{
                //    return JsonResultCommon.KhongDuQuyen("Bạn không có quyền xem thông tin tài khoản này");
                //}

                // Lấy thông tin user theo ID
                var customer = await _service.GetUserById(id);

                if (customer == null)
                    return JsonResultCommon.KhongTonTai("Không tìm thấy người dùng");

                // Kiểm tra quyền
                bool visible = !_authService.IsReadOnlyPermit("1", user.Username);

                return JsonResultCommon.ThanhCong(customer, visible);
            }


            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpPost("createAccount")]
        public async Task<object> CreateAccount(AccountModel accountModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                if (accountModel.PartnerId == 0)
                {
                    long PartnerId = long.Parse(GeneralService.GetObjectDB($"select PartnerID from AccountList where RowID = {user.Id}", _connectionString).ToString());
                    accountModel.PartnerId = PartnerId;
                }

                string sqlCheckUsername = $"select Username from AccountList where Username = '{accountModel.Username}'";
                bool isExistUsername = GeneralService.IsExistDB(sqlCheckUsername, _connectionString);
                if (isExistUsername) return JsonResultCommon.Trung("Username");

                var create = await _service.CreateAccount(accountModel, user.Id);
                if (!create.Susscess)
                {
                    return JsonResultCommon.ThatBai(create.ErrorMessgage);
                }

                return JsonResultCommon.ThanhCong(accountModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpPost("updateAccount")]
        public async Task<object> UpdateAccount(AccountModel accountModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select RowId from AccountList where RowId = {accountModel.RowId}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("account");

                var update = await _service.UpdateAccount(accountModel, user.Id);
                if (!update.Susscess)
                {
                    return JsonResultCommon.ThatBai(update.ErrorMessgage);
                }
                return JsonResultCommon.ThanhCong(accountModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpPost("UpdateUserRoles/{userId}")]
        public async Task<object> UpdateUserRoles(int userId, [FromBody] List<RoleDTO> roles)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user == null)
                    return JsonResultCommon.BatBuoc("Đăng nhập");

                // Kiểm tra tồn tại user cần cập nhật
                string sqlCheckUser = $"SELECT RowID FROM AccountList WHERE RowID = {userId}";
                bool isExistUser = GeneralService.IsExistDB(sqlCheckUser, _connectionString);
                if (!isExistUser)
                    return JsonResultCommon.KhongTonTai("Không tìm thấy user");

                // Gọi service để cập nhật roles cho user
                var result = await _service.UpdateUserRoles(userId, roles, user.Id);
                if (!result.Susscess)
                    return JsonResultCommon.ThatBai(result.ErrorMessgage);

                return JsonResultCommon.ThanhCong();
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpPost("UpdateStatusAccount")]
        public async Task<object> UpdateStatusAccount(AccountStatusModel accountModel)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select RowId from AccountList where RowId = {accountModel.RowID}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("Username");

                var update = await _service.UpdateStatusAccount(accountModel, user.Id);
                if (!update.Susscess)
                {
                    return JsonResultCommon.ThatBai(update.ErrorMessgage);
                }
                return JsonResultCommon.ThanhCong(accountModel);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpGet("GetAccountByRowID")]
        public async Task<object> GetAccountByRowID(int RowID)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var create = await _service.GetOneModelByRowID(RowID);
                if (create.RowId == 0)
                {
                    return JsonResultCommon.KhongTonTai("Account");
                }

                return JsonResultCommon.ThanhCong(create);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpGet("GetNoteLock")]
        public async Task<object> GetNoteLock(int RowID)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var note = await _service.GetNoteLock(RowID);
                return JsonResultCommon.ThanhCong(note);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpGet("GetFilterPartner")]
        public async Task<object> GetFilterPartner()
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");
                if (!user.IsMasterAccount) return JsonResultCommon.BatBuoc("User phải là isMasterAccount");

                var list = await _partnerService.GetPartnerFilters();
                if (list is null)
                {
                    return JsonResultCommon.ThatBai("Danh sách rỗng");
                }
                return JsonResultCommon.ThanhCong(list);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
    }
}