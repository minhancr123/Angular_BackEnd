using AngularBackEnd.Models.InventoryManagement;
using DpsLibs.Data;
using JeeBeginner.Models.AccountManagement;
using JeeBeginner.Models.Common;

namespace AngularBackEnd.Services.InventoryMangement
{
    public interface IIventoryManagementService
    {

        //ItemType
        public Task<IEnumerable<ItemTypeDTO>> GetItemTypeList(SqlConditions conds, string? orderByStr = null);
        Task<ReturnSqlModel> CreateItemType(ItemTypeModel itemTypeModel, long CreatedBy);
        Task<ReturnSqlModel> UpdateItemType(ItemTypeModel itemTypeModel, long ModifiedBy);
        Task<ReturnSqlModel> DeleteItemType(int id, int userId);

        //Item
        public Task<IEnumerable<ItemDTO>> GetItemList(SqlConditions conds, string? orderByStr = null);
        public Task<ReturnSqlModel> CreateItem(ItemModel itemModel, int id);
        public Task<IEnumerable<ItemDTO>> GetItemById(string id);

        Task<ReturnSqlModel> UpdateItem(ItemModel itemModel, long ModifiedBy);
        Task<ReturnSqlModel> DeleteItem(int id, int userId);

        //DVT
        public Task<IEnumerable<DVTDTO>> GetDVTList(SqlConditions conds, string? orderByStr = null);

        public Task<ReturnSqlModel> CreateDVT(DVTModel itemModel, int id);

        Task<ReturnSqlModel> UpdateDVT(DVTModel itemModel, long ModifiedBy);
        Task<ReturnSqlModel> DeleteDVT(int id, int userId);
        //Brand
        public Task<IEnumerable<BrandDTO>> GetBrandList(SqlConditions conds, string? orderByStr = null);
        public Task<ReturnSqlModel> CreateBrand(BrandModel brandModel, int id);

        Task<ReturnSqlModel> UpdateBrand(BrandModel itemModel, long ModifiedBy);
        Task<ReturnSqlModel> DeleteBrand (int id, int userId);

        //Origin
        public Task<IEnumerable<OriginModel>> GetOriginList(SqlConditions conds, string? orderByStr = null);
        public Task<ReturnSqlModel> CreateOrigin(OriginModel originModel, int id);

        Task<ReturnSqlModel> UpdateOrigin(OriginModel itemModel, long ModifiedBy);
        Task<ReturnSqlModel> DeleteOrigin(int id, int userId);

        //Insurance Partner
        public Task<IEnumerable<InsurancePartnerDTO>> GetDoitacBaohiemList(SqlConditions conds, string? orderByStr = null);
        public Task<ReturnSqlModel> CreateDoitacBaohiem(InsurancePartnerModel itemModel, int id);

        Task<ReturnSqlModel> UpdateDoitacBaohiem(InsurancePartnerModel itemModel, long ModifiedBy);
        Task<ReturnSqlModel> DeleteDoitacBaohiem(int id, int userId);
    }

}
