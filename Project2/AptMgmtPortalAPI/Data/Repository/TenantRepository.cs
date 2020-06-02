﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AptMgmtPortalAPI.Entity;
using AptMgmtPortalAPI.Data;
using AptMgmtPortalAPI.Types;
using AptMgmtPortalAPI.DataModel;

namespace AptMgmtPortalAPI.Repository
{
    public class TenantRepository : ITenant
    {
        private readonly AptMgmtDbContext _context;

        public TenantRepository(AptMgmtDbContext aptMgmtDbContext)
        {
            _context = aptMgmtDbContext;
        }

        public async Task<Tenant> AddTenant(Types.TenantInfo info)
        {
            if (info == null) return null;

            var tenant = new Tenant();
            tenant.FirstName = info.FirstName;
            tenant.LastName = info.LastName;
            tenant.Email = info.Email;
            tenant.PhoneNumber = info.PhoneNumber;
            tenant.UserId = info.UserId;

            _context.Add(tenant);
            await _context.SaveChangesAsync();
            return tenant;
        }


        public async Task<bool> EditPersonalInfo(int tenantId, Types.TenantInfo info)
        {
            if (info == null) return false;

            var tenant = await TenantFromId(tenantId);
            tenant.FirstName = info.FirstName;
            tenant.LastName = info.LastName;
            tenant.Email = info.Email;
            tenant.PhoneNumber = info.PhoneNumber;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<DataModel.Bill> GetBill(int tenantId,
                                                  ResourceType resource,
                                                  BillingPeriod period)
        {
            if (period == null) return null;

            var billingRate = await _context.ResourceUsageRates
                                        .Where(r => r.ResourceType == resource)
                                        .Where(r => r.PeriodStart <= period.PeriodStart
                                                    && r.PeriodEnd >= period.PeriodEnd)
                                        .Select(r => r)
                                        .FirstOrDefaultAsync();

            var totalUsage = await _context.TenantResourceUsages
                                 .Where(u => u.TenantId == tenantId)
                                 .Where(u => u.ResourceType == resource)
                                 .Where(u => u.SampleTime >= period.PeriodStart
                                             && u.SampleTime <= period.PeriodEnd)
                                 .SumAsync(u => u.UsageAmount);

            var totalPaid = await _context.Payments
                                         .Where(p => p.TenantId == tenantId)
                                         .Where(p => p.ResourceType == resource)
                                         .Where(p => p.BillingPeriodId == period.BillingPeriodId)
                                         .SumAsync(p => p.Amount);

            var bill = new DataModel.Bill
            {
                Resource = resource,
                Period = period,
                Usage = totalUsage,
                Rate = billingRate.Rate,
                Paid = totalPaid,
            };

            return bill;
        }

        public async Task<IEnumerable<DataModel.Bill>> GetBills(int tenantId, BillingPeriod period)
        {
            if (period == null) return new List<DataModel.Bill>();

            var billingRates = await _context.ResourceUsageRates
                                        .Where(r => r.PeriodStart <= period.PeriodStart
                                                    && r.PeriodEnd >= period.PeriodEnd)
                                        .Select(r => r)
                                        .ToListAsync();

            var totalUsages = await _context.TenantResourceUsages
                                        .Where(u => u.TenantId == tenantId)
                                        .Where(u => u.SampleTime >= period.PeriodStart
                                                    && u.SampleTime <= period.PeriodEnd)
                                        .GroupBy(u => u.ResourceType)
                                        .Select(gr => new
                                        {
                                            Resource = gr.Key,
                                            Usage = gr.Sum(u => u.UsageAmount)
                                        })
                                        .ToListAsync();

            var totalPayments = await _context.Payments
                                        .Where(p => p.TenantId == tenantId)
                                        .Where(p => p.BillingPeriodId == period.BillingPeriodId)
                                        .GroupBy(p => p.ResourceType)
                                        .Select(gr => new
                                        {
                                            Resource = gr.Key,
                                            Payment = gr.Sum(p => p.Amount),
                                        })
                                        .ToListAsync();


            var bills = new List<DataModel.Bill>();

            foreach (var resourceUsage in totalUsages)
            {
                var rate = billingRates
                            .Where(r => r.ResourceType == resourceUsage.Resource)
                            .Select(r => r.Rate)
                            .FirstOrDefault();

                var payment = totalPayments
                                .Where(p => p.Resource == resourceUsage.Resource)
                                .Sum(p => p.Payment);

                var bill = new DataModel.Bill
                {
                    Resource = resourceUsage.Resource,
                    Period = period,
                    Usage = resourceUsage.Usage,
                    Rate = rate,
                    Paid = payment,
                };

                bills.Add(bill);
            }

            return bills;
        }


        public async Task<Payment> GetPayment(int tenantId, int paymentId)
        {
            return await _context.Payments
                                 .Where(p => p.TenantId == tenantId)
                                 .Where(p => p.PaymentId == paymentId)
                                 .Select(p => p)
                                 .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Payment>> GetPayments(int tenantId,
                                                            ResourceType resource,
                                                            BillingPeriod period)
        {
            if (period == null) return new List<Payment>();

            return await _context.Payments
                                 .Where(p => p.TenantId == tenantId)
                                 .Where(p => p.ResourceType == resource)
                                 .Where(p => p.BillingPeriodId == period.BillingPeriodId)
                                 .Select(p => p)
                                 .ToListAsync();
        }

        public async Task<DataModel.TenantResourceUsageSummary> GetResourceUsage(int tenantId,
                                                                 ResourceType resource,
                                                                 BillingPeriod period)
        {
            if (period == null) return null;

            var usage = await _context.TenantResourceUsages
                                 .Where(u => u.TenantId == tenantId)
                                 .Where(u => u.ResourceType == resource)
                                 .Where(u => u.SampleTime >= period.PeriodStart && u.SampleTime <= period.PeriodEnd)
                                 .SumAsync(u => u.UsageAmount);
            return new DataModel.TenantResourceUsageSummary
            {
                ResourceType = resource,
                Usage = usage,
            };
        }

        public async Task<IEnumerable<DataModel.TenantResourceUsageSummary>> GetResourceUsage(int tenantId,
                                                                                              BillingPeriod period)
        {
            if (period == null) return new List<DataModel.TenantResourceUsageSummary>();

            return await _context.TenantResourceUsages
                    .Where(u => u.TenantId == tenantId)
                    .Where(u => u.SampleTime >= period.PeriodStart && u.SampleTime <= period.PeriodEnd)
                    .GroupBy(u => u.ResourceType)
                    .Select(gr => new DataModel.TenantResourceUsageSummary
                    {
                        ResourceType = gr.Key,
                        Usage = gr.Sum(i => i.UsageAmount),
                    })
                    .ToListAsync();
        }


        public async Task<bool> PayBill(int tenantId,
                                        double amount,
                                        ResourceType resource,
                                        BillingPeriod period)
        {
            if (period == null) return false;

            var payment = new Payment();
            payment.Amount = amount;
            payment.ResourceType = resource;
            payment.BillingPeriodId = period.BillingPeriodId;
            payment.TenantId = tenantId;
            payment.TimePaid = DateTime.Now;

            await _context.AddAsync(payment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> PayBill(int tenantId,
                                        double amount,
                                        ResourceType resource,
                                        int billingPeriodId)
        {
            var billingPeriod = await _context.BillingPeriods
                                        .Where(p => p.BillingPeriodId == billingPeriodId)
                                        .Select(p => p)
                                        .FirstOrDefaultAsync();
            return await PayBill(tenantId, amount, resource, billingPeriod);
        }

        public async Task<Tenant> TenantFromId(int tenantId)
        {
            return await _context.Tenants
                                 .Where(t => t.TenantId == tenantId)
                                 .Select(t => t)
                                 .FirstOrDefaultAsync();
        }

        public async Task<Tenant> TenantFromUserId(int userId)
        {
            return await _context.Tenants
                                 .Where(t => t.UserId == userId)
                                 .Select(t => t)
                                 .FirstOrDefaultAsync();
        }

        public async Task<int?> TenantIdFromUserId(int userId)
        {
            return await _context.Tenants
                                 .Where(t => t.UserId == userId)
                                 .Select(t => t.TenantId)
                                 .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Bill>> GetBills(int tenantId, int billingPeriodId)
        {
            var billingPeriod = await _context.BillingPeriods
                                    .Where(p => p.BillingPeriodId == billingPeriodId)
                                    .FirstOrDefaultAsync();

            return await GetBills(tenantId, billingPeriod);
        }


        public async Task<IEnumerable<Tenant>> FindTenantWithFirstName(string firstName)
        {
            if (String.IsNullOrEmpty(firstName)) return new List<Tenant>();

            firstName = firstName.ToLower();
            return await _context.Tenants
                        .Where(t => t.FirstName.ToLower().Contains(firstName))
                        .Select(t => t)
                        .ToListAsync();
        }

        public async Task<string> GetUnitNumber(int tenantId)
        {
            return await _context.Units
                .Where(u => u.TenantId == tenantId)
                .Select(u => u.UnitNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<Tenant> UpdateTenantInfo(int tenantId, DTO.TenantInfoDTO newInfo)
        {
            var tenant = await TenantFromId(tenantId);

            if (tenant == null) return null;

            tenant.FirstName = newInfo.FirstName;
            tenant.LastName = newInfo.LastName;
            tenant.Email = newInfo.Email;
            tenant.PhoneNumber = newInfo.PhoneNumber;

            await _context.SaveChangesAsync();

            return tenant;
        }
    }
}