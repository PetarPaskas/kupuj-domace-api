using System.Globalization;

namespace KupujDomace.Services;

/// <summary>Ports services/currency_service.py: static EUR/RSD rates from configuration.</summary>
public class CurrencyService
{
    private const double DefaultEurToRsd = 117.0;
    private const double DefaultRsdToEur = 0.00855;

    private readonly double _eurToRsd;
    private readonly double _rsdToEur;

    public CurrencyService(IConfiguration config)
    {
        _eurToRsd = ParseRate(config["EUR_TO_RSD_RATE"], DefaultEurToRsd);
        _rsdToEur = ParseRate(config["RSD_TO_EUR_RATE"], DefaultRsdToEur);
    }

    private static double ParseRate(string? value, double fallback) =>
        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var r) ? r : fallback;

    public Doc GetRates() => new()
    {
        ["EUR_TO_RSD"] = _eurToRsd,
        ["RSD_TO_EUR"] = _rsdToEur,
    };

    public double Convert(double amount, string fromCurrency, string toCurrency)
    {
        fromCurrency = fromCurrency.ToUpperInvariant();
        toCurrency = toCurrency.ToUpperInvariant();

        if (fromCurrency == toCurrency)
            return amount;
        if (fromCurrency == "EUR" && toCurrency == "RSD")
            return Math.Round(amount * _eurToRsd, 2);
        if (fromCurrency == "RSD" && toCurrency == "EUR")
            return Math.Round(amount * _rsdToEur, 2);

        throw new InvalidOperationException($"Unsupported currency conversion: {fromCurrency} to {toCurrency}");
    }

    public string FormatPrice(double amount, string currency, string language = "en")
    {
        currency = currency.ToUpperInvariant();
        var value = amount.ToString("F2", CultureInfo.InvariantCulture);

        if (currency == "EUR")
            return language == "sr" ? $"{value} €" : $"€{value}";
        if (currency == "RSD")
            return $"{value} RSD";

        return $"{value} {currency}";
    }
}
