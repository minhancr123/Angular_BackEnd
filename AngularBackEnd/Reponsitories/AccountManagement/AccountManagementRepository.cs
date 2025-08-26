using AngularBackEnd.Models.AccountManagement;
using DpsLibs.Data;
using JeeBeginner.Classes;
using JeeBeginner.Models.AccountManagement;
using JeeBeginner.Models.Common;
using JeeBeginner.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace JeeBeginner.Reponsitories.AccountManagement
{
    public class AccountManagementRepository : IAccountManagementRepository
    {
        private readonly string _connectionString;

        public AccountManagementRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ReturnSqlModel> CreateAccount(AccountModel model, long CreatedBy)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var val = InitDataAccount(model, CreatedBy);
                    int x = cnn.Insert(val, "AccountList");
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

        private Hashtable InitDataAccount(AccountModel account, long CreatedBy, bool isUpdate = false)
        {
            Hashtable val = new Hashtable();
            val.Add("PartnerID", account.PartnerId);
            val.Add("Fullname", account.Fullname);
            val.Add("Mobile", account.Mobile);
            val.Add("Email", account.Email);
            val.Add("Username", account.Username);
            val.Add("Password", DpsLibs.Common.EncDec.Encrypt(account.Password, Constant.PASSWORD_ED));
            val.Add("IsLock", 0);
            val.Add("Gender", account.Gender);
            val.Add("Note", account.Note);
            val.Add("IsMasterAccount", 1);
            if (!isUpdate)
            {
                val.Add("CreatedDate", DateTime.UtcNow);
                val.Add("CreatedBy", CreatedBy);
            }
            return val;
        }

        public async Task<IEnumerable<AccountDTO>> GetAll(SqlConditions conds, string orderByStr)
        {
            DataTable dt = new DataTable();
            string sql = "";
            if (conds.Count == 0)
            {
                sql = $@"select AccountList.*, PartnerList.PartnerName 
                        from AccountList
                        join PartnerList 
                        on AccountList.PartnerID = PartnerList.RowID 
                        order by {orderByStr}";
            }
            else
            {
                sql = $@"select AccountList.*, PartnerList.PartnerName 
                        from AccountList
                        join PartnerList 
                        on AccountList.PartnerID = PartnerList.RowID 
                        where (where) order by {orderByStr}";
            }
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = cnn.CreateDataTable(sql, "(where)", conds);
                var result = dt.AsEnumerable().Select(row => new AccountDTO
                {
                    Username = row["Username"].ToString(),
                    Fullname = row["Fullname"].ToString(),
                    Mobile = row["Mobile"].ToString(),
                    IsLock = Convert.ToBoolean((bool)row["IsLock"]),
                    RowId = Int32.Parse(row["RowID"].ToString()),
                    CreatedDate = (row["CreatedDate"] != DBNull.Value) ? ((DateTime)row["CreatedDate"]).ToString("dd/MM/yyyy") : "",
                    PartnerName = row["PartnerName"].ToString(),
                    LastLogin = (row["LastLogin"] != DBNull.Value) ? ((DateTime)row["LastLogin"]).ToString("dd/MM/yyyy HH:mm:ss") : "",
                });
                return await Task.FromResult(result);
            }
        }
        public async Task<AccountModel> GetOneModelByRowID(int RowID)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            Conds.Add("RowID", RowID);
            string sql = @"select * from AccountList where RowID = @RowID";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                string Username = dt.Rows[0]["Username"].ToString();
                string Password = DpsLibs.Common.EncDec.Decrypt(dt.Rows[0]["Password"].ToString(), Constant.PASSWORD_ED);
                var result = dt.AsEnumerable().Select(row => new AccountModel
                {
                    Gender = row["Gender"].ToString(),
                    Fullname = row["Fullname"].ToString(),
                    Email = row["Email"].ToString(),
                    Mobile = row["Mobile"].ToString(),
                    Note = row["Note"].ToString(),
                    PartnerId = Int32.Parse(row["PartnerId"].ToString()),
                    RowId = Int32.Parse(row["RowID"].ToString()),
                    Username = Username,
                    Password = Password
                }).SingleOrDefault();
                return await Task.FromResult(result);
            }
        }

        public async Task<ReturnSqlModel> UpdateAccount(AccountModel model, long CreatedBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("RowID", model.RowId);
                    val = InitDataAccount(model, CreatedBy, true);
                    int x = cnn.Update(val, conds, "AccountList");
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
        public async Task<string> GetNoteLock(long RowID)
        {
            DataTable dt = new DataTable();
            SqlConditions conds = new SqlConditions();
            conds.Add("RowID", RowID);
            string result = "";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                string sql = "select LockedNote, UnlockedNote, IsLock from AccountList where RowID = @RowID";
                dt = await cnn.CreateDataTableAsync(sql, conds);
                bool isLock = (bool)dt.Rows[0]["IsLock"];
                if (isLock)
                {
                    result = dt.Rows[0]["UnlockedNote"].ToString();
                }
                else
                {
                    result = dt.Rows[0]["LockedNote"].ToString();
                }
                return await Task.FromResult(result);
            }
        }
        public async Task<ReturnSqlModel> UpdateStatusAccount(AccountStatusModel model, long CreatedBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("RowID", model.RowID);
                    if (model.IsLock)
                    {
                        val.Add("IsLock", 0);
                        val.Add("UnlockedNote", model.Note);
                        val.Add("UnlockedBy", CreatedBy);
                        val.Add("UnlockedDate", DateTime.UtcNow);
                    }
                    else
                    {
                        val.Add("IsLock", 1);
                        val.Add("LockedNote", model.Note);
                        val.Add("LockedBy", CreatedBy);
                        val.Add("LockedDate", DateTime.UtcNow);
                    }
                    int x = cnn.Update(val, conds, "AccountList");
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

        public async Task<IEnumerable<AccountRole>> GetUserById(int id)
        {
            SqlConditions Conds = new SqlConditions();
            Conds.Add("RowID", id);

            string sql = @"
        SELECT AccountList.*, Tbl_Permision.*, Tbl_Account_Permit.Edit ,Tbl_Account_Permit.Visible
        FROM AccountList
        JOIN Tbl_Account_Permit ON Tbl_Account_Permit.Username = AccountList.Username
        JOIN Tbl_Permision ON Tbl_Account_Permit.Id_Permit = Tbl_Permision.Id_permit
        WHERE AccountList.RowID = @RowID";

            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                var dt = await cnn.CreateDataTableAsync(sql, Conds);

                if (dt.Rows.Count == 0)
                    return null; // hoặc throw lỗi nếu cần

                if (dt.Rows.Count == 0)
                    return new List<AccountRole>(); // trả list rỗng thay vì null

                string username = dt.Rows[0]["Username"].ToString();
                string fullname = dt.Rows[0]["Fullname"].ToString();
                string mobile = dt.Rows[0]["Mobile"].ToString();
                int rowId = int.Parse(dt.Rows[0]["RowID"].ToString());


                // Lấy thông tin user (chỉ từ dòng đầu tiên)
                var accountRole = dt.AsEnumerable().Select(row => new AccountRole
                {
                    Fullname = fullname,
                    Mobile = mobile,
                    Id_Permit = Convert.ToByte(row["Id_permit"].ToString()),
                    Username = username,
                    Tenquyen = row["Tenquyen"].ToString(),
                    Description = row["Description"].ToString(),
                    Edit = row["Edit"] != DBNull.Value && Convert.ToBoolean(row["Edit"]),
                    Visible = row["Visible"] != DBNull.Value && Convert.ToBoolean(row["Visible"])
                }).ToList();

                return accountRole;
            }
        }

        public Task<(bool Susscess, string ErrorMessgage)> UpdateUserRoles(int userId, List<RoleDTO> roles, int updatedBy)
        {
            try
            {
                using (var db = new DpsConnection(_connectionString))
                {
                    db.BeginTransaction();

                    // 1. Lấy Username từ AccountList
                    string getUserSql = "SELECT Username FROM AccountList WHERE RowID = @UserId";
                    var dtUser = db.CreateDataTable(getUserSql, new SqlConditions { { "UserId", userId } });

                    if (dtUser.Rows.Count == 0)
                    {
                        db.RollbackTransaction();
                        return Task.FromResult<(bool Susscess, string ErrorMessgage)>((false, "Không tìm thấy Username"));
                    }

                    string username = dtUser.Rows[0]["Username"].ToString();

                    // 2. Lặp qua roles để update hoặc insert
                    foreach (var role in roles)
                    {
                        string roleId = role.Id_Permit;
                        int editValue = role.viewOnly ? 0 : 1;
                        int Visible = role.Edit ? 1 : 0;
                        // Kiểm tra tồn tại
                        string checkSql = "SELECT COUNT(*) AS Cnt FROM Tbl_Account_Permit WHERE Username = @Username AND Id_Permit = @IdPermit";
                        var dtCheck = db.CreateDataTable(checkSql, new SqlConditions
                {
                    { "Username", username },
                    { "IdPermit", roleId }
                });

                        int exists = Convert.ToInt32(dtCheck.Rows[0]["Cnt"]);

                        if (exists > 0)
                        {
                            // Update Edit = editValue
                            string updateSql = @"UPDATE Tbl_Account_Permit
                                         SET Edit = @Edit
                                         WHERE Username = @Username AND Id_Permit = @IdPermit";
                            if (db.ExecuteNonQuery(updateSql, new SqlConditions
                    {
                        { "Edit", editValue },
                        { "Username", username },
                        { "IdPermit", roleId }
                    }) <= 0)
                            {
                                db.RollbackTransaction();
                                return Task.FromResult<(bool Susscess, string ErrorMessgage)>((false, $"Không thể update role {roleId}"));
                            }
                            // Update Visible
                            //        string updateSql1 = @"UPDATE MainMenu
                            //                     SET Visible = @Visible
                            //                     WHERE  PermissionID = @IdPermit";
                            //        if (db.ExecuteNonQuery(updateSql1, new SqlConditions
                            //{
                            //    { "Visible", Visible },
                            //    { "IdPermit", roleId }
                            //}) <= 0)
                            //        {
                            //            db.RollbackTransaction();
                            //            return Task.FromResult<(bool Susscess, string ErrorMessgage)>((false, $"Không thể update role {roleId}"));
                            //        }

                            // Update Visible = VisibleValue
                            string updateSql1 = @"UPDATE Tbl_Account_Permit
                                         SET Visible = @Visible
                                         WHERE Username = @Username AND Id_Permit = @IdPermit";
                            if (db.ExecuteNonQuery(updateSql1, new SqlConditions
                    {
                        { "Visible", Visible },
                        { "Username", username },
                        { "IdPermit", roleId }
                    }) <= 0)
                            {
                                db.RollbackTransaction();
                                return Task.FromResult<(bool Susscess, string ErrorMessgage)>((false, $"Không thể update role {roleId}"));
                            }
                        }
                        else
                        {
                            // Insert mới
                            string insertSql = @"INSERT INTO Tbl_Account_Permit (Username, Id_Permit, Edit, Id_chucnang,Visible)
                                         VALUES (@Username, @IdPermit, @Edit, NULL , @Visible)";
                            if (db.ExecuteNonQuery(insertSql, new SqlConditions
                    {
                        { "Username", username },
                        { "IdPermit", roleId },
                        { "Edit", editValue },
                        {"Visible", Visible }
                    }) <= 0)
                            {
                                db.RollbackTransaction();
                                return Task.FromResult<(bool Susscess, string ErrorMessgage)>((false, $"Không thể thêm role {roleId}"));
                            }


                        }
                    }

                    db.EndTransaction();
                    return Task.FromResult<(bool Susscess, string ErrorMessgage)>((true, ""));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult<(bool Susscess, string ErrorMessgage)>((false, ex.Message));
            }
        }



    }
}