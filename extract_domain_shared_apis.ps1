#!/usr/bin/env pwsh

# Extract API signatures from Domain.Shared build errors
$buildOutput = @"
/Users/seco/Documents/GitHub/pikachu_blazor/src/Kooco.Pikachu.Domain.Shared/AntBlazorModels/Upload/UploadState.cs(7,9): warning RS0016: 符號 'Uploading' 並非宣告的公用 API 的一部分
/Users/seco/Documents/GitHub/pikachu_blazor/src/Kooco.Pikachu.Domain.Shared/EnumValues/EnumType.cs(6,9): warning RS0016: 符號 'ShippingMethod' 並非宣告的公用 API 的一部分
/Users/seco/Documents/GitHub/pikachu_blazor/src/Kooco.Pikachu.Domain.Shared/EnumValues/EnumType.cs(7,9): warning RS0016: 符號 'TaxType' 並非宣告的公用 API 的一部分
/Users/seco/Documents/GitHub/pikachu_blazor/src/Kooco.Pikachu.Domain.Shared/EnumValues/FreebieOrderReach.cs(10,9): warning RS0016: 符號 'MinimumPiece' 並非宣告的公用 API 的一部分
/Users/seco/Documents/GitHub/pikachu_blazor/src/Kooco.Pikachu.Domain.Shared/AntBlazorModels/Upload/UploadState.cs(6,9): warning RS0016: 符號 'Fail' 並非宣告的公用 API 的一部分
/Users/seco/Documents/GitHub/pikachu_blazor/src/Kooco.Pikachu.Domain.Shared/EnumValues/GroupBuyModuleType.cs(6,5): warning RS0016: 符號 'ProductImageModule' 並非宣告的公用 API 的一部分
/Users/seco/Documents/GitHub/pikachu_blazor/src/Kooco.Pikachu.Domain.Shared/EnumValues/GroupBuyModuleType.cs(7,5): warning RS0016: 符號 'ProductGroupModule' 並非宣告的公用 API 的一部分
/Users/seco/Documents/GitHub/pikachu_blazor/src/Kooco.Pikachu.Domain.Shared/EnumValues/GroupBuyModuleType.cs(5,5): warning RS0016: 符號 'ProductDescriptionModule' 並非宣告的公用 API 的一部分
/Users/seco/Documents/GitHub/pikachu_blazor/src/Kooco.Pikachu.Domain.Shared/EnumValues/GroupBuyModuleType.cs(11,5): warning RS0016: 符號 'GroupPurchaseOverview' 並非宣告的公用 API 的一部分
/Users/seco/Documents/GitHub/pikachu_blazor/src/Kooco.Pikachu.Domain.Shared/EnumValues/GroupBuyModuleType.cs(10,5): warning RS0016: 符號 'BannerImages' 並非宣告的公用 API 的一部分
/Users/seco/Documents/GitHub/pikachu_blazor/src/Kooco.Pikachu.Domain.Shared/AntBlazorModels/Upload/UploadState.cs(5,9): warning RS0016: 符號 'Success' 並非宣告的公用 API 的一部分
/Users/seco/Documents/GitHub/pikachu_blazor/src/Kooco.Pikachu.Domain.Shared/EnumValues/GroupBuyModuleType.cs(12,5): warning RS0016: 符號 'CountdownTimer' 並非宣告的公用 API 的一部分
/Users/seco/Documents/GitHub/pikachu_blazor/src/Kooco.Pikachu.Domain.Shared/EnumValues/GroupBuyModuleType.cs(8,5): warning RS0016: 符號 'CarouselImages' 並非宣告的公用 API 的一部分
/Users/seco/Documents/GitHub/pikachu_blazor/src/Kooco.Pikachu.Domain.Shared/EnumValues/FreebieOrderReach.cs(9,9): warning RS0016: 符號 'MinimumAmount' 並非宣告的公用 API 的一部分
/Users/seco/Documents/GitHub/pikachu_blazor/src/Kooco.Pikachu.Domain.Shared/EnumValues/GroupBuyModuleType.cs(14,5): warning RS0016: 符號 'ProductRankingCarouselModule' 並非宣告的公用 API 的一部分
/Users/seco/Documents/GitHub/pikachu_blazor/src/Kooco.Pikachu.Domain.Shared/EnumValues/GroupBuyModuleType.cs(3,13): warning RS0016: 符號 'GroupBuyModuleType' 並非宣告的公用 API 的一部分
/Users/seco/Documents/GitHub/pikachu_blazor/src/Kooco.Pikachu.Domain.Shared/AntBlazorModels/Upload/UploadState.cs(3,17): warning RS0016: 符號 'UploadState' 並非宣告的公用 API 的一部分
"@

# Extract unique API signatures
$apiSignatures = @()
$buildOutput -split "`n" | ForEach-Object {
    if ($_ -match "符號\s+'([^']+)'.*並非宣告的公用 API") {
        $apiSignatures += $matches[1]
    }
}

# Get unique signatures and sort them
$uniqueApis = $apiSignatures | Sort-Object | Get-Unique

Write-Host "找到 $($uniqueApis.Count) 個缺失的 API 簽名:"
$uniqueApis | ForEach-Object { Write-Host "  $_" }

# Output them in PublicAPI format
Write-Host "`n格式化的 API 簽名:"
$uniqueApis | ForEach-Object { Write-Host $_ }