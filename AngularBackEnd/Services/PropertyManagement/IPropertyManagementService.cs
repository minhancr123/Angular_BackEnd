using AngularBackEnd.Models.InventoryManagement;
using AngularBackEnd.Models.PropertyManagement;
using DpsLibs.Data;
using JeeBeginner.Models.AccountManagement;
using JeeBeginner.Models.Common;

namespace AngularBackEnd.Services.PropertyMangement
{
    public interface IPropertyManagementService
    {

        //PropertyType
        public Task<IEnumerable<PropertyType>> GetPropertyTypeList(SqlConditions conds, string? orderByStr = null);
        Task<ReturnSqlModel> CreatePropertyType(PropertyType propertyTypeModel, long CreatedBy);
        Task<ReturnSqlModel> UpdatePropertyType(PropertyType propertyTypeModel, long ModifiedBy);
        Task<ReturnSqlModel> DeletePropertyType(int id, int userId);

        //GroupPropertyType
        public Task<IEnumerable<GroupPropertyType>> GetGroupPropertyTypeList(SqlConditions conds, string? orderByStr = null);
        Task<ReturnSqlModel> CreateGroupPropertyType(GroupPropertyType groupPropertyTypeModel, long CreatedBy);
        Task<ReturnSqlModel> UpdateGroupPropertyType(GroupPropertyType groupPropertyTypeModel, long ModifiedBy);
        Task<ReturnSqlModel> DeleteGroupPropertyType(int id, int userId);


        //Ly do tang giam tai san
        public Task<IEnumerable<Reason>> GetReasonList(SqlConditions conds, string? orderByStr = null);
        Task<ReturnSqlModel> CreateReason(Reason reasonModel, long CreatedBy);
        Task<ReturnSqlModel> UpdateReason(Reason reasonModel, long ModifiedBy);
        Task<ReturnSqlModel> DeleteReason(int id, int userId);
    }

}
