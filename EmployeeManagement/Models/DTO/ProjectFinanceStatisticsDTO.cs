using System;
using System.Collections.Generic;

namespace EmployeeManagement.Models.DTO
{
    /// <summary>
    /// Thống kê tài chính dự án
    /// </summary>
    /// 
    public class ProjectFinanceStatistics
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
        public decimal Budget { get; set; }
        public decimal BudgetUtilization { get; set; }
        public int TransactionCount { get; set; }
        public DateTime? LastTransactionDate { get; set; }
    }


    public class BudgetCheckResult
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public decimal Budget { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingBudget { get; set; }
        public decimal BudgetUtilization { get; set; }
        public bool IsOverBudget { get; set; }
        public bool IsNearBudgetLimit { get; set; }
        public string Status { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
    }



    /// <summary>
    /// Kết quả tính ROI dự án
    /// </summary>
    public class ProjectROIResult
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public decimal TotalInvestment { get; set; }
        public decimal TotalReturn { get; set; }
        public decimal ROI { get; set; }
        public decimal ROIPercentage { get; set; }
        public string ROICategory { get; set; }
        public DateTime CalculationDate { get; set; }
    }
    /// <summary>
    /// Tài chính dự án theo tháng
    /// </summary>
    public class MonthlyProjectFinance
    {
        public int ProjectID { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
    public decimal Balance { get; set; }
    public int TransactionCount { get; set; }
}
/// <summary>
/// Bộ lọc tài chính dự án
/// </summary>
public class ProjectFinanceFilter
    {
        public string TransactionType { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string SearchText { get; set; }
        public int? RecordedByID { get; set; }
        public List<string> ExcludeStatuses { get; set; } = new List<string>();
    }

    /// <summary>
    /// Phân tích ngân sách
    /// </summary>
    public class BudgetAnalysis
    {
        public decimal PlannedBudget { get; set; }
        public decimal ActualSpent { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercent { get; set; }
        public string PerformanceRating { get; set; }
        public List<BudgetCategoryAnalysis> CategoryAnalysis { get; set; }
        public List<string> Insights { get; set; }

        public string VarianceDisplay => Variance >= 0 ? $"+{Variance:#,##0}" : $"{Variance:#,##0}";
        public string VarianceClass => Variance >= 0 ? "text-danger" : "text-success";
    }

    /// <summary>
    /// Phân tích ngân sách theo danh mục
    /// </summary>
    public class BudgetCategoryAnalysis
    {
        public string Category { get; set; }
        public decimal PlannedAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercent { get; set; }
    }

    /// <summary>
    /// Tóm tắt theo danh mục
    /// </summary>
    public class CategorySummary
    {
        public string Category { get; set; }
        public string CategoryDisplay { get; set; }
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
        public decimal Percentage { get; set; }

        public string AmountDisplay => Amount.ToString("#,##0.00") + " VNĐ";
        public string PercentageDisplay => Percentage.ToString("F1") + "%";
    }

    /// <summary>
    /// Thống kê tài chính với DTO đầy đủ
    /// </summary>
    public class FinanceStatistics
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
        public int TotalTransactions { get; set; }
        public decimal IncomePercentage { get; set; }
        public decimal ExpensePercentage { get; set; }
        public decimal ProfitMargin { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    /// <summary>
    /// Dự báo tài chính
    /// </summary>
    public class FinanceForecast
    {
        public List<MonthlyForecast> MonthlyForecasts { get; set; }
        public decimal PredictedTotalIncome { get; set; }
        public decimal PredictedTotalExpense { get; set; }
        public decimal PredictedBalance { get; set; }
        public string ConfidenceLevel { get; set; }
        public List<string> Assumptions { get; set; }
        public DateTime ForecastDate { get; set; }
    }

   
    /// <summary>
    /// Phân tích hiệu suất
    /// </summary>
    public class PerformanceAnalysis
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public FinanceStatistics CurrentPeriod { get; set; }
        public FinanceStatistics ComparisonPeriod { get; set; }
        public decimal IncomeChange { get; set; }
        public decimal ExpenseChange { get; set; }
        public decimal ProfitabilityChange { get; set; }
        public string OverallPerformance { get; set; }
        public List<string> KeyInsights { get; set; }
        public List<PerformanceMetric> Metrics { get; set; }

        public string IncomeChangeDisplay => IncomeChange >= 0 ? $"+{IncomeChange:F1}%" : $"{IncomeChange:F1}%";
        public string ExpenseChangeDisplay => ExpenseChange >= 0 ? $"+{ExpenseChange:F1}%" : $"{ExpenseChange:F1}%";
        public string IncomeChangeClass => IncomeChange >= 0 ? "text-success" : "text-danger";
        public string ExpenseChangeClass => ExpenseChange <= 0 ? "text-success" : "text-warning";
    }

    /// <summary>
    /// Chỉ số hiệu suất
    /// </summary>
    public class PerformanceMetric
    {
        public string MetricName { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal PreviousValue { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercent { get; set; }
        public string Status { get; set; }

        public string ChangeDisplay => Change >= 0 ? $"+{Change:#,##0}" : $"{Change:#,##0}";
        public string ChangePercentDisplay => ChangePercent >= 0 ? $"+{ChangePercent:F1}%" : $"{ChangePercent:F1}%";
        public string StatusClass => Status switch
        {
            "Improved" => "text-success",
            "Declined" => "text-danger",
            "Stable" => "text-info",
            _ => "text-secondary"
        };
    }

    /// <summary>
    /// Kết quả phát hiện bất thường
    /// </summary>
    public class AnomalyDetectionResult
    {
        public int FinanceID { get; set; }
        public string TransactionCode { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string AnomalyType { get; set; }
        public decimal Severity { get; set; }
        public string Description { get; set; }
        public List<string> Reasons { get; set; }
        public decimal Confidence { get; set; }

        public string SeverityDisplay => Severity switch
        {
            >= 0.8m => "🔴 Cao",
            >= 0.6m => "🟡 Trung bình",
            >= 0.4m => "🟢 Thấp",
            _ => "⚪ Rất thấp"
        };

        public string SeverityClass => Severity switch
        {
            >= 0.8m => "text-danger",
            >= 0.6m => "text-warning",
            >= 0.4m => "text-info",
            _ => "text-secondary"
        };

        public string ConfidenceDisplay => $"{Confidence * 100:F1}%";
    }

    /// <summary>
    /// Báo cáo cash flow nâng cao
    /// </summary>
    public class AdvancedCashFlowReport
    {
        public List<CashFlowReport> MonthlyFlows { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal AverageMonthlyIncome { get; set; }
        public decimal AverageMonthlyExpense { get; set; }
        public decimal CashFlowVolatility { get; set; }
        public int PositiveFlowMonths { get; set; }
        public int NegativeFlowMonths { get; set; }
        public decimal MaxMonthlyInflow { get; set; }
        public decimal MaxMonthlyOutflow { get; set; }
        public string CashFlowTrend { get; set; }
        public List<string> Recommendations { get; set; }
    }

    /// <summary>
    /// Phân tích theo nhóm khách hàng
    /// </summary>
    public class CustomerGroupAnalysis
    {
        public string GroupName { get; set; }
        public int CustomerCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageRevenuePerCustomer { get; set; }
        public decimal RevenuePercentage { get; set; }
        public List<TopCustomer> TopCustomers { get; set; }

        public string RevenueDisplay => TotalRevenue.ToString("#,##0.00") + " VNĐ";
        public string AvgRevenueDisplay => AverageRevenuePerCustomer.ToString("#,##0.00") + " VNĐ";
        public string RevenuePercentDisplay => RevenuePercentage.ToString("F1") + "%";
    }

    /// <summary>
    /// Top khách hàng
    /// </summary>
    public class TopCustomer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TransactionCount { get; set; }
        public DateTime LastTransactionDate { get; set; }

        public string RevenueDisplay => TotalRevenue.ToString("#,##0.00") + " VNĐ";
        public string LastTransactionDisplay => LastTransactionDate.ToString("dd/MM/yyyy");
    }

    /// <summary>
    /// Phân tích seasonal (theo mùa)
    /// </summary>
    public class SeasonalAnalysis
    {
        public List<QuarterlyAnalysis> QuarterlyData { get; set; }
        public List<MonthlySeasonality> MonthlyPatterns { get; set; }
        public string PeakSeason { get; set; }
        public string LowSeason { get; set; }
        public decimal SeasonalityIndex { get; set; }
        public List<string> SeasonalInsights { get; set; }
    }

    /// <summary>
    /// Phân tích theo quý
    /// </summary>
    public class QuarterlyAnalysis
    {
        public int Year { get; set; }
        public int Quarter { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal NetProfit { get; set; }
        public decimal GrowthRate { get; set; }

        public string QuarterDisplay => $"Q{Quarter}/{Year}";
        public string GrowthRateDisplay => GrowthRate >= 0 ? $"+{GrowthRate:F1}%" : $"{GrowthRate:F1}%";
        public string GrowthRateClass => GrowthRate >= 0 ? "text-success" : "text-danger";
    }

    /// <summary>
    /// Tính mùa vụ theo tháng
    /// </summary>
    public class MonthlySeasonality
    {
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal AverageIncome { get; set; }
        public decimal AverageExpense { get; set; }
        public decimal SeasonalityFactor { get; set; }
        public string Pattern { get; set; }

        public string SeasonalityDisplay => SeasonalityFactor.ToString("F2");
        public string PatternClass => Pattern switch
        {
            "Peak" => "text-success",
            "High" => "text-info",
            "Average" => "text-secondary",
            "Low" => "text-warning",
            "Valley" => "text-danger",
            _ => "text-secondary"
        };
    }

    /// <summary>
    /// Cảnh báo tài chính
    /// </summary>
    public class FinanceAlert
    {
        public string AlertType { get; set; }
        public string Severity { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public DateTime AlertDate { get; set; }
        public bool IsActive { get; set; }
        public List<string> Actions { get; set; }
        public Dictionary<string, object> AlertData { get; set; }

        public string SeverityDisplay => Severity switch
        {
            "Critical" => "🚨 Nghiêm trọng",
            "High" => "⚠️ Cao",
            "Medium" => "🟡 Trung bình",
            "Low" => "ℹ️ Thấp",
            _ => Severity
        };

        public string SeverityClass => Severity switch
        {
            "Critical" => "alert-danger",
            "High" => "alert-warning",
            "Medium" => "alert-info",
            "Low" => "alert-secondary",
            _ => "alert-light"
        };
    }

    /// <summary>
    /// Benchmark so sánh
    /// </summary>
    public class BenchmarkComparison
    {
        public string MetricName { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal BenchmarkValue { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercent { get; set; }
        public string Performance { get; set; }
        public string BenchmarkSource { get; set; }

        public string VarianceDisplay => Variance >= 0 ? $"+{Variance:#,##0}" : $"{Variance:#,##0}";
        public string VariancePercentDisplay => VariancePercent >= 0 ? $"+{VariancePercent:F1}%" : $"{VariancePercent:F1}%";
        public string PerformanceClass => Performance switch
        {
            "Excellent" => "text-success",
            "Good" => "text-info",
            "Average" => "text-warning",
            "Poor" => "text-danger",
            _ => "text-secondary"
        };
    }

    /// <summary>
    /// Phân tích rủi ro tài chính
    /// </summary>
    public class FinancialRiskAnalysis
    {
        public decimal LiquidityRisk { get; set; }
        public decimal ConcentrationRisk { get; set; }
        public decimal VolatilityRisk { get; set; }
        public decimal OverallRiskScore { get; set; }
        public string RiskLevel { get; set; }
        public List<RiskFactor> RiskFactors { get; set; }
        public List<string> Mitigation { get; set; }

        public string RiskLevelDisplay => RiskLevel switch
        {
            "Low" => "🟢 Thấp",
            "Medium" => "🟡 Trung bình",
            "High" => "🟠 Cao",
            "Critical" => "🔴 Nghiêm trọng",
            _ => RiskLevel
        };

        public string RiskLevelClass => RiskLevel switch
        {
            "Low" => "text-success",
            "Medium" => "text-info",
            "High" => "text-warning",
            "Critical" => "text-danger",
            _ => "text-secondary"
        };
    }

    /// <summary>
    /// Yếu tố rủi ro
    /// </summary>
    public class RiskFactor
    {
        public string FactorName { get; set; }
        public decimal Score { get; set; }
        public string Impact { get; set; }
        public string Description { get; set; }
        public List<string> Indicators { get; set; }

        public string ScoreDisplay => $"{Score:F1}/10";
        public string ImpactClass => Impact switch
        {
            "Low" => "text-success",
            "Medium" => "text-warning",
            "High" => "text-danger",
            _ => "text-secondary"
        };
    }
}