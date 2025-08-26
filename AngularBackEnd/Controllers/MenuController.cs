using DpsLibs.Data;
using JeeAccount.Classes;
using JeeBeginner.Classes;
using JeeBeginner.Models.Common;
using JeeBeginner.Reponsitories.Authorization;
using JeeBeginner.Services.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace JeeBeginner.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/menu")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly string _jwtSecret;
        private readonly string _connectionString;
        private readonly ICustomAuthorizationService _authService;

        public MenuController(IConfiguration configuration, ICustomAuthorizationService authService)
        {
            _authService = authService;
            _config = configuration;
            _jwtSecret = configuration.GetValue<string>("JWT:Secret");
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region Load menu

        /// <summary>
        /// Load menu
        /// </summary>
        /// <returns></returns>

        [Route("LayMenuChucNang")]
        [HttpGet]
        public object LayMenuChucNang()
        {
            ErrorModel error = new ErrorModel();
            DataSet ds = new DataSet();

            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null)
                {
                    return Unauthorized(MessageReturnHelper.DangNhap());
                }

                string sql_listRole = "";
                SqlConditions cond = new SqlConditions();
                cond.Add("HienThi", 1);

                List<long> listrole = _authService.GetRules(user.Username);
                for (int i = 0; i < listrole.Count; i++)
                {
                    sql_listRole += ",@IDRole" + i;
                    cond.Add("IDRole" + i, listrole[i]);
                }
                if (!string.IsNullOrEmpty(sql_listRole))
                    sql_listRole = sql_listRole.Substring(1);
                if (listrole.Count == 0)
                {
                    sql_listRole = "0"; // không có quyền nào
                }

                // Subquery chỉ lấy code menu có hiển thị
                string subQueryMainMenuCodes = @"
            select code
            from MainMenu  
            where HienThi=@HienThi
        ";

                // Lấy MainMenu theo PermissionID (lọc theo role user)
                string select_MainMenu = $@"
            select mm.code, mm.title, mm.PermissionID, mm.Target, mm.Visible,
                   mm.Summary, isNULL(mm.ALink, '#') as ALink, 
                   ISNULL(mm.Icon, 'flaticon-interface-7') as Icon, 
                   '' as title_, mm.position
            from MainMenu mm
            inner join ({subQueryMainMenuCodes}) t on mm.code = t.code 
            where mm.Visible = 1
              and (mm.PermissionID in ({sql_listRole}) or mm.PermissionID is null)
            order by mm.position
        ";

                // Lấy SubMenu theo AllowPermit (lọc theo role user)
                string select_Menu = $@"
            select sm.title, sm.AllowPermit, sm.Target, sm.id_row,
                   sm.GroupName, sm.ALink, sm.Summary, sm.AppLink, sm.AppIcon, '' as title_
            from Tbl_Submenu sm
            inner join ({subQueryMainMenuCodes}) t on sm.GroupName = t.code
            where (sm.AllowPermit in ({sql_listRole}) or sm.AllowPermit is null)
              and sm.HienThi=@HienThi
              and sm.CustemerID is null
            order by sm.position
        ";

                // Ghép query
                string sql = select_MainMenu + ";" + select_Menu;

                using (DpsConnection cnn = new DpsConnection(_connectionString))
                {
                    ds = cnn.CreateDataSet(sql, cond);
                    if (ds.Tables.Count == 0)
                        return JsonResultCommon.ThatBai("Không có dữ liệu", cnn.LastError);
                }

                // Ghép MainMenu + SubMenu thành JSON
                var data = from r in ds.Tables[0].AsEnumerable()
                           select new
                           {
                               Code = r["Code"].ToString(),
                               Title = r["title"].ToString(),
                               Target = r["Target"],
                               Summary = r["Summary"].ToString(),
                               Icon = r["Icon"].ToString(),
                               ALink = r["ALink"].ToString(),
                               Visible = r.Field<bool?>("Visible") == true ? 1 : 0,
                               Child = from c in ds.Tables[1].AsEnumerable()
                                       where c["GroupName"].ToString().Trim().ToLower()
                                             == r["Code"].ToString().Trim().ToLower()
                                       select new
                                       {
                                           Title = c["title"].ToString(),
                                           Summary = c["Summary"].ToString(),
                                           AllowPermit = c["AllowPermit"]?.ToString(),
                                           Target = c["Target"].ToString(),
                                           GroupName = c["GroupName"].ToString(),
                                           ALink = c["ALink"].ToString(),
                                       },
                           };

                return JsonResultCommon.ThanhCong(data);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }


        #endregion Load menu
    }
}