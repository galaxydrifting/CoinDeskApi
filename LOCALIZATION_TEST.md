# 多語系功能測試指南

## 🧪 測試方法

### 1. Query String（推薦）
```bash
# 英文
curl "http://localhost:5253/api/currencies?culture=en-US"

# 中文  
curl "http://localhost:5253/api/currencies?culture=zh-TW"
```

### 2. Accept-Language Header
```bash
# 英文
curl -H "Accept-Language: en-US" http://localhost:5253/api/currencies

# 中文
curl -H "Accept-Language: zh-TW" http://localhost:5253/api/currencies
```

### 3. Cookie
```bash
# 英文
curl --cookie ".AspNetCore.Culture=c=en-US|uic=en-US" http://localhost:5253/api/currencies

# 中文
curl --cookie ".AspNetCore.Culture=c=zh-TW|uic=zh-TW" http://localhost:5253/api/currencies
```

## ✅ 預期回應

**英文**: `{"success":true,"message":"Currencies retrieved successfully",...}`  
**中文**: `{"success":true,"message":"成功取得幣別列表",...}`

## ⚙️ 設定說明

### 語言選擇優先順序
1. Query String (`?culture=zh-TW`)
2. Cookie (`.AspNetCore.Culture=c=zh-TW|uic=zh-TW`)
3. Accept-Language Header

### Cookie 格式說明
- 格式：`.AspNetCore.Culture=c=zh-TW|uic=zh-TW`
- `c=`: Culture (數字、日期格式)
- `uic=`: UI Culture (多語系文字)

### Postman 測試
1. **Accept-Language**: Headers → `Accept-Language: zh-TW`
2. **Query String**: URL → `?culture=zh-TW`
3. **Cookie**: Headers → `Cookie: .AspNetCore.Culture=c=zh-TW|uic=zh-TW`
