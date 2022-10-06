using System.Net;
using ECommerce.Contracts.Dtos.Seller;
using ECommerce.Contracts.Interfaces.Repositories;
using ECommerce.Contracts.Interfaces.Services;

namespace ECommerce.Api.Services;

public class SellerService : ISellerService
{
    private readonly ISellerRepository _sellerRepository;

    public SellerService(ISellerRepository sellerRepository)
    {
        _sellerRepository = sellerRepository;
    }

    public async Task<(SellerDto?, HttpStatusCode)> CreateSeller(CreateSellerDto sellerDto)
    {
        var (sellerModel, statusCode) = await _sellerRepository.CreateSeller(sellerDto.ToModel());
        return (SellerDto.FromModel(sellerModel), statusCode);
    }

    public Task<HttpStatusCode> DeleteSeller(Guid sellerId)
        => _sellerRepository.DeleteSeller(sellerId);

    public async Task<(SellerDto?, HttpStatusCode)> GetSellerById(Guid sellerId)
    {
        var (sellerModel, statusCode) = await _sellerRepository.GetSellerById(sellerId);
        return (SellerDto.FromModel(sellerModel), statusCode);
    }

    public Task<HttpStatusCode> UpdateSeller(Guid sellerId, UpdateSellerDto seller)
    {
        var sellerModel = seller.ToModel();
        sellerModel.Id = sellerId;

        return _sellerRepository.UpdateSeller(sellerModel);
    }
}