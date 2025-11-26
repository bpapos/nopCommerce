using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.BSSDataUpgrades;

[NopSchemaMigration("2025-11-25 00:00:00", "Product. Add monthly license")]
public class AddProductMonthlyFee : ForwardOnlyMigration
{
    /// <summary>
    /// Add monthly fee fields
    /// </summary>
    public override void Up()
    {
        var productTableName = nameof(Product);
        if (!Schema.Table(productTableName).Column(nameof(Product.HasMonthlyLicense)).Exists())
            Alter.Table(productTableName)
                .AddColumn(nameof(Product.HasMonthlyLicense)).AsBoolean().NotNullable().SetExistingRowsTo(false);

        if (!Schema.Table(productTableName).Column(nameof(Product.MonthlyLicenseFee)).Exists())
            Alter.Table(productTableName)
                .AddColumn(nameof(Product.MonthlyLicenseFee)).AsDecimal().NotNullable().SetExistingRowsTo(0);
    }
}
