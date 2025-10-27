using Application.Features.Interface;
using Domain.Entities;

namespace Application.Features.Services;

public class TransactionHistoryService : ITransactionHistoryService
{
    private readonly ILichSuGiaoDich _transactionRepository;

    public TransactionHistoryService(ILichSuGiaoDich transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<IEnumerable<LichSuGiaoDich>> GetAllAsync()
    {
        return await _transactionRepository.GetAll();
    }

    public async Task<LichSuGiaoDich?> GetByIdAsync(int id)
    {
        return await _transactionRepository.GetById(id);
    }

    public async Task<IEnumerable<LichSuGiaoDich>> GetByUserIdAsync(int maUser)
    {
        var allTransactions = await _transactionRepository.GetAll();
        return allTransactions.Where(t => t.HoaDon?.MaUser == maUser);
    }

    public async Task<IEnumerable<LichSuGiaoDich>> GetByHoaDonIdAsync(int maHD)
    {
        var allTransactions = await _transactionRepository.GetAll();
        return allTransactions.Where(t => t.MaHD == maHD);
    }

    public async Task AddAsync(LichSuGiaoDich transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        if (transaction.SoTien <= 0)
            throw new ArgumentException("Số tiền phải lớn hơn 0");

        if (string.IsNullOrWhiteSpace(transaction.Provider))
            throw new ArgumentException("Provider không được để trống");

        if (string.IsNullOrWhiteSpace(transaction.TrangThai))
            throw new ArgumentException("Trạng thái không được để trống");

        await _transactionRepository.Add(transaction);
    }

    public async Task UpdateAsync(LichSuGiaoDich transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        await _transactionRepository.Update(transaction);
    }
}

