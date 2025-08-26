using AngularBackEnd.Models.InventoryManagement;
using AngularBackEnd.Models.PropertyManagement;
using AngularBackEnd.Reponsitories.PropertyMangement;
using AngularBackEnd.Services.PropertyMangement;
using DpsLibs.Data;
using JeeBeginner.Models.AccountManagement;
using JeeBeginner.Models.Common;
using JeeBeginner.Reponsitories.AccountManagement;
using System.Threading.Tasks;

namespace AngularBackEnd.Services.InventoryMangement
{
    public class PropertyManagementService : IPropertyManagementService
    {
        private IPropertyMangementRepository _reposiory;

        public PropertyManagementService(IPropertyMangementRepository repository)
        {
            _reposiory = repository;
        }

        //PropertyType
        public async Task<IEnumerable<PropertyType>> GetPropertyTypeList(SqlConditions conds, string? orderByStr = null)
        {
            return await _reposiory.GetPropertyTypeList(conds, orderByStr);
        }

        public async Task<ReturnSqlModel> CreatePropertyType(PropertyType propertyTypeModel, long CreatedBy)
        {
            return await _reposiory.CreatePropertyType(propertyTypeModel, CreatedBy);

        }

    

        public async Task<ReturnSqlModel> DeletePropertyType(int id, int userId)
        {
            return await _reposiory.DeletePropertyType(id, userId);
        }





        public async Task<ReturnSqlModel> UpdatePropertyType(PropertyType propertyTypeModel, long ModifiedBy)
        {
            return await _reposiory.UpdatePropertyType(propertyTypeModel, ModifiedBy);

        }

        //Group Property Type
        public async Task<IEnumerable<GroupPropertyType>> GetGroupPropertyTypeList(SqlConditions conds, string? orderByStr = null)
        {
            return await _reposiory.GetGroupPropertyTypeList(conds, orderByStr);
        }

        public async Task<ReturnSqlModel> CreateGroupPropertyType(GroupPropertyType groupPropertyTypeModel, long CreatedBy)
        {
            return await _reposiory.CreateGroupPropertyType(groupPropertyTypeModel, CreatedBy);
        }

        public async Task<ReturnSqlModel> UpdateGroupPropertyType(GroupPropertyType groupPropertyTypeModel, long ModifiedBy)
        {
            return await _reposiory.UpdateGroupPropertyType(groupPropertyTypeModel, ModifiedBy);
        }

        public async Task<ReturnSqlModel> DeleteGroupPropertyType(int id, int userId)
        {
            return await _reposiory.DeleteGroupPropertyType(id, userId);
        }

        public Task<ReturnSqlModel> UpdateGroupPropertyType(PropertyType groupPropertyTypeModel, long ModifiedBy)
        {
            throw new NotImplementedException();
        }

        //Ly do tang giam tai san
        public async Task<IEnumerable<Reason>> GetReasonList(SqlConditions conds, string? orderByStr = null)
        {
            return await _reposiory.GetReasonList(conds, orderByStr);
        }

        public async Task<ReturnSqlModel> CreateReason(Reason reasonModel, long CreatedBy)
        {
            return await _reposiory.CreateReason(reasonModel, CreatedBy);
        }

        public async Task<ReturnSqlModel> UpdateReason(Reason reasonModel, long ModifiedBy)
        {
            return await _reposiory.UpdateReason(reasonModel, ModifiedBy);
        }

        public async Task<ReturnSqlModel> DeleteReason(int id, int userId)
        {
            return await _reposiory.DeleteReason(id, userId);
        }
    }
}
