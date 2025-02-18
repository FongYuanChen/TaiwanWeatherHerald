# ![icon](https://github.com/user-attachments/assets/4f50ab17-d8f0-4bab-8738-d4880f9b8e03) 台灣天氣報馬仔 ![GitHub watchers](https://img.shields.io/github/watchers/FongYuanChen/TaiwanWeatherHerald) ![GitHub forks](https://img.shields.io/github/forks/FongYuanChen/TaiwanWeatherHerald) ![GitHub Repo stars](https://img.shields.io/github/stars/FongYuanChen/TaiwanWeatherHerald) ![GitHub last commit](https://img.shields.io/github/last-commit/FongYuanChen/TaiwanWeatherHerald) ![GitHub License](https://img.shields.io/github/license/FongYuanChen/TaiwanWeatherHerald)

**天氣預報更精準，台灣旅遊愛好者的救星！**

> 是否曾因錯誤的天氣預報讓精心規劃的行程泡湯？天氣預報的準確度直接影響了旅遊體驗，而本專案致力於改善這個問題，讓你無憂遊遍台灣每個角落！

> 本專案旨在學習WPF，並透過開發一款專為台灣旅遊設計的天氣APP，提升實戰開發經驗。


## 🎨 功能特色

- **台灣旅遊景點全面整合**：涵蓋休閒農場、國家公園、國家風景區、國家森林遊樂區、水庫等台灣熱門景點，讓旅遊規劃更精準！
- **多維度天氣資訊**：提供72小時天氣預報、一週天氣趨勢，並搭配天氣小幫手與氣象圖資，資訊一應俱全！
- **即時影像驗證**：監視器影像串流，讓你親眼確認目的地天氣，而不只是看數據！
- **簡單易用**：直覺式操作介面，無需繁瑣設定，開啟即用！


## 🔗 數據來源

- **天氣數據**：基於 [交通部中央氣象署-氣象資料開發平台](https://opendata.cwa.gov.tw/index)，獲取天氣預報、天氣圖資等數據。
- **監視器資訊頁面**：基於 [台灣即時影像監視器API](https://www.twipcam.com/api/document)，獲取經緯度座標附近的監視器資訊頁面。


## 🛠 核心技術

- **語言框架**：基於 C#、.NET 8、WPF。
- **非同步API整合**：基於 HttpClient 進行非同步資料擷取。
- **HTML解析 & 網頁爬取**：基於 [HtmlAgilityPack](https://html-agility-pack.net/) 解析監視器資訊頁面，自動擷取即時影像網址。
- **地圖可視化**：基於 [GMap.NET](https://github.com/judero01col/GMap.NET) 整合地圖標記，顯示監視器位置，讓用戶可視化查詢即時影像來源。
- **瀏覽器內嵌**：基於 [WebView2](https://aka.ms/webview) 提供流暢的即時影像。


## 🖥️ 操作演示

https://github.com/user-attachments/assets/306c6aae-ec2c-4a44-a2f1-140e9aed7aa1


## 📜 授權條款

本專案採用 [MIT](https://github.com/FongYuanChen/TaiwanWeatherHerald/blob/main/LICENSE) 授權條款。歡迎自由使用、修改與分享！
