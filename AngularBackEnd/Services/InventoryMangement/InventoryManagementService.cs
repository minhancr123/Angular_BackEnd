using AngularBackEnd.Models.InventoryManagement;
using AngularBackEnd.Reponsitories.InventoryMangement;
using DpsLibs.Data;
using JeeBeginner.Models.AccountManagement;
using JeeBeginner.Models.Common;
using JeeBeginner.Reponsitories.AccountManagement;
using System.Threading.Tasks;

namespace AngularBackEnd.Services.InventoryMangement
{
    public class InventoryManagementService : IIventoryManagementService
    {
        private IInventoryMangementRepository _repository;

        public InventoryManagementService(IInventoryMangementRepository repository)
        {
            _repository = repository;
        }
        //ItemType

        public async Task<ReturnSqlModel> CreateItemType(ItemTypeModel itemTypeModel, long CreatedBy)
        {
            return await _repository.CreateItemType(itemTypeModel, CreatedBy);
        }

        public async Task<ReturnSqlModel> DeleteItemType(int id, int userId)
        {
            return await _repository.DeleteItemType(id, userId);
        }


        public async Task<IEnumerable<ItemTypeDTO>> GetItemTypeList(SqlConditions conds, string? orderByStr = null)
        {
            return await _repository.GetItemTypeList(conds, orderByStr);
        }

        public async Task<ReturnSqlModel> UpdateItemType(ItemTypeModel itemTypeModel, long ModifiedBy)
        {
            return await _repository.UpdateItemType(itemTypeModel, ModifiedBy);
        }

        //Item
        public async Task<IEnumerable<ItemDTO>> GetItemList(SqlConditions conds, string? orderByStr = null)
        {
            return await _repository.GetItemList(conds, orderByStr);
        }
        public async Task<ReturnSqlModel> CreateItem(ItemModel itemModel, int id)
        {
            return await _repository.CreateItem(itemModel, id);
        }

        public async Task<ReturnSqlModel> UpdateItem(ItemModel itemModel, long ModifiedBy)
        {
            return await _repository.UpdateItem(itemModel, ModifiedBy);
        }

        public async Task<ReturnSqlModel> DeleteItem(int id, int userId)
        {
            return await _repository.DeleteItem(id, userId);
        }

        public async Task<IEnumerable<ItemDTO>> GetItemById(string id)
        {
            return await _repository.GetItemById(id);
        }

        //DVT
        public async Task<IEnumerable<DVTDTO>> GetDVTList(SqlConditions conds, string? orderByStr = null)
        {
            return await _repository.GetDVTList(conds, orderByStr);
        }

        public async Task<ReturnSqlModel> CreateDVT(DVTModel itemModel, int id)
        {
            return await _repository.CreateDVT(itemModel, id);
        }

        public async Task<ReturnSqlModel> UpdateDVT(DVTModel itemModel, long ModifiedBy)
        {
            return await _repository.UpdateDVT(itemModel, ModifiedBy);
        }

        public async Task<ReturnSqlModel> DeleteDVT(int id, int userId)
        {
            return await _repository.DeleteDVT(id, userId);
        }
        

        //Brand
        public async Task<IEnumerable<BrandDTO>> GetBrandList(SqlConditions conds, string? orderByStr = null)
        {
            return await _repository.GetBrandList(conds, orderByStr);
        }

        public async Task<ReturnSqlModel> CreateBrand(BrandModel brandModel, int id)
        {
           return await _repository.CreateBrand(brandModel, id);
        }

        public async Task<ReturnSqlModel> UpdateBrand(BrandModel itemModel, long ModifiedBy)
        {
            return await _repository.UpdateBrand(itemModel, ModifiedBy);
        }

        public async Task<ReturnSqlModel> DeleteBrand(int id, int userId)
        {
            return await _repository.DeleteBrand(id, userId);
        }

        //Origin
        public async Task<IEnumerable<OriginModel>> GetOriginList(SqlConditions conds, string? orderByStr = null)
        {
            return await _repository.GetOriginList(conds, orderByStr);
        }

       

        public async Task<ReturnSqlModel> UpdateOrigin(OriginModel itemModel, long ModifiedBy)
        {
            return await _repository.UpdateOrigin(itemModel, ModifiedBy);
        }

        public async Task<ReturnSqlModel> DeleteOrigin(int id, int userId)
        {
            return await _repository.DeleteOrigin(id, userId);
        }

        public async Task<ReturnSqlModel> CreateOrigin(OriginModel originModel, int id)
        {
            return await _repository.CreateOrigin(originModel, id);
        }
        public async Task<IEnumerable<InsurancePartnerDTO>> GetDoitacBaohiemList(SqlConditions conds, string? orderByStr = null)
        {
            return await _repository.GetDoitacBaohiemList(conds, orderByStr);
        }

        public async Task<ReturnSqlModel> CreateDoitacBaohiem(InsurancePartnerModel itemModel, int id)
        {
            return await _repository.CreateDoitacBaohiem(itemModel, id);
        }

        public async Task<ReturnSqlModel> UpdateDoitacBaohiem(InsurancePartnerModel itemModel, long ModifiedBy)
        {
            return await _repository.UpdateDoitacBaohiem(itemModel, ModifiedBy);
        }

        public async Task<ReturnSqlModel> DeleteDoitacBaohiem(int id, int userId)
        {
            return await _repository.DeleteDoitacBaohiem(id, userId);
        }

        
    }
}
