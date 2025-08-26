using AngularBackEnd.Models.PropertyManagement;
using DpsLibs.Data;
using JeeBeginner.Models.AccountManagement;
using JeeBeginner.Models.Common;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AngularBackEnd.Reponsitories.PropertyMangement
{
    public interface IPropertyMangementRepository
    {
        //PropertyType
        public Task<IEnumerable<PropertyType>> GetPropertyTypeList(SqlConditions conds , string? orderByStr = null);
        
        Task<ReturnSqlModel> UpdatePropertyType(PropertyType propertyTypeModel, long ModifiedBy);

        Task<ReturnSqlModel> CreatePropertyType(PropertyType propertyTypeModel, long CreatedBy) ;
        Task<ReturnSqlModel> DeletePropertyType(int id, int userId);

        //GroupPropertyType
        public Task<IEnumerable<GroupPropertyType>> GetGroupPropertyTypeList(SqlConditions conds, string? orderByStr = null);

        Task<ReturnSqlModel> UpdateGroupPropertyType(GroupPropertyType groupPropertyTypeModel, long ModifiedBy);

        Task<ReturnSqlModel> CreateGroupPropertyType(GroupPropertyType groupPropertyTypeModel, long CreatedBy);
        Task<ReturnSqlModel> DeleteGroupPropertyType(int id, int userId);

        //Ly do tang giam tai san
        public Task<IEnumerable<Reason>> GetReasonList(SqlConditions conds, string? orderByStr = null);

        Task<ReturnSqlModel> UpdateReason(Reason reasonModel, long ModifiedBy);

        Task<ReturnSqlModel> CreateReason(Reason reasonModel, long CreatedBy);
        Task<ReturnSqlModel> DeleteReason(int id, int userId);
    }
}
