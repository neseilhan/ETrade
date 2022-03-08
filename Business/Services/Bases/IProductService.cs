using AppCore.Business.Models.Paging;
using AppCore.Business.Models.Results;
using AppCore.Business.Services.Bases;
using Business.Models;
using Business.Models.Filters;
using Business.Models.Reports;
using System.Collections.Generic;

namespace Business.Services.Bases
{
    public interface IProductService : IService<ProductModel>
    {
        //[Obsolete("Bu methodun daha yeni bir versiyonu bulunmaktadır.")]
        // obsolete: kullanıldığı yerde kullanıldığı yapının daha yeni bir versiyonu olduğunu ve bu yeni versiyonun kullanılmasının gerektiğini belirtir. 
        Result<List<ProductsReportModel>> GetProductsReport(ProductsReportFilterModel filter = null, PageModel page = null, AppCore.Business.Models.Ordering.OrderModel order = null);
    }
}
