# CoinDesk API - Bitcoin Price Index Web API

## 專案概述

這是一個基於 ASP.NET Core 8.0 的 Web API 專案，整合 CoinDesk API 來提供比特幣價格指數資訊，並實作幣別管理功能。

## 技術棧

- **框架**: ASP.NET Core 8.0 Web API
- **資料庫**: SQL Server Express LocalDB
- **ORM**: Entity Framework Core
- **對象映射**: AutoMapper
- **日誌**: Serilog
- **API 文檔**: Swagger/OpenAPI
- **測試**: xUnit + Moq
- **容器化**: Docker

## 功能特色

### 核心功能
- ✅ 呼叫 CoinDesk API 取得比特幣價格資訊
- ✅ 幣別資料庫 CRUD 操作 (依幣別代碼排序)
- ✅ 資料轉換與格式化 (時間格式: yyyy/MM/dd HH:mm:ss)
- ✅ 錯誤處理與 Mocking Data 支援

### 加分功能實作
- ✅ **API 請求/回應日誌記錄**: 完整記錄所有 API 呼叫和外部 API 請求
- ✅ **Error Handling**: 全域例外處理中介軟體
- ✅ **Swagger UI**: 完整的 API 文檔介面
- ✅ **Design Pattern**: Repository Pattern, Dependency Injection
- ✅ **Docker 支援**: 可運行在 Docker 容器中
- ✅ **加解密技術**: AES 對稱加密 + RSA 非對稱加密

## API 端點

### 幣別管理
- `GET /api/currencies` - 取得所有幣別 (依代碼排序)
- `GET /api/currencies/{id}` - 取得特定幣別
- `POST /api/currencies` - 新增幣別
- `PUT /api/currencies/{id}` - 更新幣別
- `DELETE /api/currencies/{id}` - 刪除幣別

### CoinDesk 資料
- `GET /api/coindesk/original` - 取得原始 CoinDesk API 資料
- `GET /api/coindesk/transformed` - 取得轉換後的資料格式

### 加解密功能
- `POST /api/encryption/aes/encrypt` - AES 加密
- `POST /api/encryption/aes/decrypt` - AES 解密
- `POST /api/encryption/rsa/generate-keys` - 產生 RSA 金鑰對
- `POST /api/encryption/rsa/encrypt` - RSA 加密
- `POST /api/encryption/rsa/decrypt` - RSA 解密

## 資料庫設計

### Currency 資料表
```sql
CREATE TABLE Currency (
    Id NVARCHAR(10) PRIMARY KEY,      -- 幣別代碼
    ChineseName NVARCHAR(50) NOT NULL, -- 中文名稱
    EnglishName NVARCHAR(100),         -- 英文名稱
    Symbol NVARCHAR(10),               -- 符號
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE()
);

-- 預設資料
INSERT INTO Currency (Id, ChineseName, EnglishName, Symbol) VALUES
('USD', '美元', 'US Dollar', '$'),
('EUR', '歐元', 'Euro', '€'),
('GBP', '英鎊', 'British Pound Sterling', '£');
```

## 快速開始

### 環境需求
- .NET 8.0 SDK
- SQL Server Express LocalDB
- Visual Studio 2022 或 VS Code

### 本地運行

1. **克隆專案**
```bash
git clone https://github.com/galaxydrifting/CoinDeskApi.git
cd coindesk
```

2. **還原套件**
```bash
dotnet restore
```

3. **建立資料庫**
```bash
dotnet ef database update --project CoinDeskApi.Infrastructure --startup-project CoinDeskApi.Api
```

4. **啟動應用程式**
```bash
dotnet run --project CoinDeskApi.Api
```

5. **開啟 Swagger UI**
瀏覽器開啟: `https://localhost:7139` 或 `http://localhost:5139`

### Docker 運行

1. **使用 Docker Compose**
```bash
docker-compose up -d
```

2. **開啟 API**
瀏覽器開啟: `http://localhost:5000`

## 專案結構

```
CoinDeskApi/
├── CoinDeskApi.Api/                 # Web API 主專案
│   ├── Controllers/                 # API 控制器
│   ├── Middleware/                  # 中介軟體
│   └── Resources/                   # 多語系資源 (規劃中)
├── CoinDeskApi.Core/                # 核心業務邏輯
│   ├── DTOs/                        # 資料傳輸物件
│   ├── Entities/                    # 實體類別
│   ├── Interfaces/                  # 介面定義
│   └── Exceptions/                  # 自定義例外
├── CoinDeskApi.Infrastructure/      # 基礎設施層
│   ├── Data/                        # 資料庫上下文
│   ├── Repositories/                # 資料存取
│   ├── Services/                    # 業務服務
│   └── Mapping/                     # AutoMapper 配置
├── CoinDeskApi.Tests/               # 單元測試
└── Docker 相關檔案
```

## 測試

### 執行單元測試
```bash
dotnet test
```

### 測試覆蓋
- Controller 測試
- Service 層測試
- Repository 測試
- 加解密功能測試
- 外部 API 呼叫測試

## 設計模式

1. **Repository Pattern**: 抽象化資料存取邏輯
2. **Dependency Injection**: 依賴注入降低耦合
3. **Strategy Pattern**: 加解密策略模式

## 日誌記錄

使用 Serilog 進行結構化日誌記錄:
- Console 輸出
- 檔案記錄 (每日輪替)
- 請求/回應完整記錄

## 錯誤處理

- 全域例外處理中介軟體
- 統一錯誤回應格式
- 外部 API 失敗自動切換 Mocking Data
- 詳細錯誤日誌記錄

## 開發者資訊

- **開發日期**: 2025年7月28日
- **.NET 版本**: 8.0
- **資料庫**: SQL Server Express LocalDB

## 注意事項

1. 請確保 SQL Server Express LocalDB 已正確安裝
2. 首次運行會自動建立資料庫和初始資料
3. CoinDesk API 呼叫失敗時會自動使用 Mocking Data
4. 所有 API 呼叫都會被完整記錄在日誌中
