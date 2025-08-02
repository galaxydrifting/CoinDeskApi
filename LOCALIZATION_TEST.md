# 多語系功能測試指南

## 測試方法

### 1. 使用 Accept-Language Header

**測試英文版本:**
```bash
curl -H "Accept-Language: en-US" http://localhost:5253/api/currencies
```

**測試中文版本:**
```bash
curl -H "Accept-Language: zh-TW" http://localhost:5253/api/currencies
```

### 2. 使用 Query String

**測試英文版本:**
```bash
curl "http://localhost:5253/api/currencies?culture=en-US"
```

**測試中文版本:**
```bash
curl "http://localhost:5253/api/currencies?culture=zh-TW"
```

### 3. 測試錯誤訊息

**測試找不到幣別 (英文):**
```bash
curl -H "Accept-Language: en-US" http://localhost:5253/api/currencies/XXX
```

**測試找不到幣別 (中文):**
```bash
curl -H "Accept-Language: zh-TW" http://localhost:5253/api/currencies/XXX
```

**測試驗證失敗 (英文):**
```bash
curl -X POST -H "Accept-Language: en-US" -H "Content-Type: application/json" \
     -d '{"id":"","name":"","chineseName":""}' \
     http://localhost:5253/api/currencies
```

**測試驗證失敗 (中文):**
```bash
curl -X POST -H "Accept-Language: zh-TW" -H "Content-Type: application/json" \
     -d '{"id":"","name":"","chineseName":""}' \
     http://localhost:5253/api/currencies
```

## 預期回應

### 英文回應範例
```json
{
  "success": true,
  "message": "Currencies retrieved successfully",
  "data": [...],
  "errors": null
}
```

### 中文回應範例
```json
{
  "success": true,
  "message": "成功取得幣別列表",
  "data": [...],
  "errors": null
}
```

## 支援的語言
- `en-US`: 英文 (預設)
- `zh-TW`: 繁體中文

## 語言選擇優先順序
1. Query String (`?culture=zh-TW`)
2. Cookie (`.AspNetCore.Culture`)
3. Accept-Language Header
