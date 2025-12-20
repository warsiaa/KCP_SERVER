# 📡 KCP_SERVER

Bu proje, **C# (.NET)** kullanılarak geliştirilmiş, **KCP protokolü** üzerinden çalışan basit ve yüksek performanslı bir **UDP tabanlı sunucu** uygulamasıdır.  
KCP, UDP’nin hızını korurken TCP benzeri güvenilirlik sunmayı amaçlar.

Bu repo, KCP mantığını öğrenmek, test etmek ve oyun / gerçek zamanlı uygulamalarda temel bir server altyapısı kurmak için hazırlanmıştır.

---

## 🚀 Özellikler

- 🔹 KCP protokolü ile güvenilir UDP iletişimi  
- 🔹 Sunucu taraflı bağlantı yönetimi  
- 🔹 Düşük gecikme, yüksek performans  
- 🔹 Basit ve anlaşılır C# kod yapısı  
- 🔹 Oyun sunucuları ve real-time uygulamalar için uygun altyapı  

---

## 🧱 Gereksinimler

- .NET 6.0 veya üzeri  
- Visual Studio 2022 (önerilir)  
- UDP bağlantılarına izin veren ağ ortamı  

---

## 📦 Kurulum

```bash
git clone https://github.com/warsiaa/KCP_SERVER.git
cd KCP_SERVER
```

Visual Studio ile:
1. `KCP_SERVER.slnx` aç  
2. Build  
3. Run  

CLI:
```bash
dotnet build KCP_SERVER.slnx
dotnet run
```

---

## 🧠 Genel Mantık

- UDP socket dinlenir  
- Her client için KCP session tutulur  
- Update döngüsü ile paketler işlenir  

---

## 📡 Örnek Kullanım

```csharp
var server = new KcpServer(7777);
server.Start();
```

```csharp
while (true)
{
    server.Update();
    Thread.Sleep(1);
}
```

---

## 📄 Lisans

MIT License

---

## ⚠️ Not

Bu proje temel altyapıdır.  
Prod ortamı için ek güvenlik katmanları eklenmelidir.
