# GrandNode External Order Import API
Bu proje, GrandNode a��k kaynak e-ticaret altyap�s�na harici sistemlerden sipari� aktar�m� i�in geli�tirilmi� bir API endpoint i�erir. `/api/order` endpoint�i �zerinden POST istekleri ile sipari� verisi al�n�r, do�rulan�r ve GrandNode veri taban�na kaydedilir.

## �zellikler

- **POST /api/order** endpoint�i ile sipari� alma
- JSON format�nda sipari� verisi bekler
- M��teri bilgilerini do�rular, gerekirse yeni guest m��teri olu�turur
- �r�n SKU bilgilerini kontrol eder, bulunamayan �r�n varsa hata d�ner
- Sipari� ve sipari� kalemlerini GrandNode�a kaydeder
- Hatal� isteklerde detayl� ve anlaml� hata mesajlar� d�ner
- Ba�ar�l� i�lemlerde sipari� numaras� (OrderNumber) d�ner
- Hatalar GrandNode log sistemine kaydedilir


### Request �rne�i
{
    "page": 0,
    "size": 50,
    "totalPages": 1,
    "totalElements": 1,
    "content": [
        {
            "shipmentAddress": {
                "id": 80844024,
                "firstName": "Trendyol",
                "lastName": "M��terisi",
                "company": "",
                "address1": "DSM Grup Dan��manl�k �leti�im ve Sat�� Tic. A.�. B�y�kdere Caddesi Noramin �� Merkezi No:237 Kat:B1 ",
                "address2": "",
                "addressLines": {
                    "addressLine1": "xxx",
                    "addressLine2": "xxx"
                },
                "city": " �stanbul ",
                "cityCode": 34,
                "district": "�i�li",
                "districtId": 54,
                "countyId": 0, 
                "countyName": "XXX",
                "shortAddress": "xxx",
                "stateName": "xxx",
                "postalCode": "10D",
                "countryCode": "TR",
                "neighborhoodId": 32126,
                "neighborhood": "Beyaz�t Mah",
                "phone": null,
                "fullName": "Trendyol M��terisi",
                "fullAddress": "DSM Grup Dan��manl�k �leti�im ve Sat�� Tic. A.�. B�y�kdere Caddesi Noramin �� Merkezi No:237 Kat:B1   �i�li  �stanbul "
            },
            "orderNumber": "80869231",
            "grossAmount": 51.98,
            "totalDiscount": 25.99,
            "totalTyDiscount": 0.00,
            "taxNumber": null,
            "invoiceAddress": {
                "id": 80844023,
                "firstName": "Trendyol",
                "lastName": "M��terisi",
                "company": "", // GULF b�lgesi sipari�lerinde bo� gelebilir.
                "address1": "DSM Grup Dan��manl�k �leti�im ve Sat�� Tic. A.�. B�y�kdere Caddesi Noramin �� Merkezi No:237 Kat:B1 ",
                "address2": "", // GULF b�lgesi sipari�lerinde bo� gelebilir.
                "addressLines": {
                    "addressLine1": "xxx",
                    "addressLine2": "xxx"
                },
                "city": " �stanbul ",
                "district": "�i�li", // GULF b�lgesi sipari�lerinde bo� gelebilir.
                "districtId": 1234,
                "countyId": 0, // CEE b�lgesi i�in gelecektir.
                "countyName": "XXX", // CEE b�lgesi i�in gelecektir.
                "shortAddress": "xxx", // GULF b�lgesi i�in gelecektir.
                "stateName": "xxx", // GULF b�lgesi i�in gelecektir.
                "postalCode": "", // GULF b�lgesi sipari�lerinde bo� gelebilir.
                "countryCode": "TR",
                "neighborhoodId": 32126,
                "neighborhood": "Beyaz�t Mah",
                "phone": null,
                "fullName": "Trendyol M��terisi",
                "fullAddress": "DSM Grup Dan��manl�k �leti�im ve Sat�� Tic. A.�. B�y�kdere Caddesi Noramin �� Merkezi No:237 Kat:B1   �i�li  �stanbul",
                "taxOffice": "Company of OMS's Tax Office", 
                "taxNumber": "Company of OMS's Tax Number" 
            },
            "customerFirstName": "Trendyol",
            "customerEmail": "pf+dym24k@trendyolmail.com",
            "customerId": 99993706,
            "customerLastName": "M��terisi",
            "id": 11650604, //shipmentPackageId
            "cargoTrackingNumber": 7340447182689,
            "cargoTrackingLink": "https://kargotakip.trendyol.com/?token=",
            "cargoSenderNumber": "733861966410",
            "cargoProviderName": "Trendyol Express Marketplace",
            "lines": [
                {
                    "quantity": 2,
                    "salesCampaignId": 201642,
                    "productSize": " one size",
                    "merchantSku": "merchantSku",
                    "productName": "Kad�n �ivit Mavi Geometrik Desenli Kapakl� Clutch sku1234 sku1234, one size",
                    "productCode": 11954798,
                    "productOrigin": "Tr",
                    "merchantId": 201,
                    "amount": 25.99,
                    "discount": 13.00,
                    "tyDiscount": 0.00,
                    "discountDetails": [
                        {
                            "lineItemPrice": 13.00,
                            "lineItemDiscount": 12.99,
                            "lineItemTyDiscount": 0.00 
                        },
                        {
                            "lineItemPrice": 12.99,
                            "lineItemDiscount": 13.00,
                            "lineItemTyDiscount": 0.00 
                        }
                    ],
                    "fastDeliveryOptions": [
                        {
                            "type": "SameDayShipping"
                        },
                        {
                            "type": "FastDelivery"
                        }
                    ],
                    "currencyCode": "TRY",
                    "productColor": "No Color",
                    "id": 56040534, // orderLineId
                    "sku": "sku1234",
                    "vatBaseAmount": 8,
                    "barcode": "barcode1234",
                    "orderLineItemStatusName": "ReturnAccepted",
                    "price": 12.99,
                    "productCategoryId": 11111,
                    "laborCost": 11.11
                }
            ],
            "orderDate": 1542801149863,
            "tcIdentityNumber": "99999999999",
            "identityNumber": "0000000000000",
            "currencyCode": "TRY",
            "packageHistories": [
                {
                    "createdDate": 1542790350607,
                    "status": "Created"
                },
                {
                    "createdDate": 1543789070462,
                    "status": "Delivered"
                },
                {
                    "createdDate": 1542872460911,
                    "status": "Picking"
                },
                {
                    "createdDate": 1542953901874,
                    "status": "Shipped"
                }
            ],
            "shipmentPackageStatus": "ReturnAccepted",
            "status": "Shipped",
            "deliveryType": "normal", 
            "timeSlotId": 0,
            "scheduledDeliveryStoreId": "",
            "estimatedDeliveryStartDate": 1614605119000,
            "estimatedDeliveryEndDate": 1615296319000,
            "totalPrice": 469.90,
            "deliveryAddressType": "Shipment",
            "agreedDeliveryDate": 1622549842955,
            "agreedDeliveryDateExtendible": true,
            "extendedAgreedDeliveryDate": 1615296319000, 
            "agreedDeliveryExtensionStartDate": 1615296319000, 
            "agreedDeliveryExtensionEndDate": 1615296319000,
            "invoiceLink": "",
            "fastDelivery": true,
            "fastDeliveryType": "FastDelivery",
            "originShipmentDate": 1542790350607, 
            "lastModifiedDate": 1641210225935,
            "commercial": true,
            "deliveredByService": false,
            "micro": true,
            "giftBoxRequested": true, 
            "etgbNo": "243414X001232",
            "etgbDate": 1705089600000,
            "3pByTrendyol": false,
            "containsDangerousProduct": true 
        }
    ]
}

### Response �rnekleri
Ba�ar�l� Sipari� Olu�umu (200 OK)

{
  "IsSuccess": true,
  "OrderId": "686427bfc811d64a3e2763a8",
  "Message": "Order has been created successfully"
}

Ba�ar�s�z Sipari� Olu�umu (400 badrequest)

{
  "IsSuccess": false,
  "OrderId": "686427bfc811d64a3e2763a8",
  "Message": "ilgili hata mesaj�"
}
Validasyon Hatalar� (400 badrequest)
{
  "Content": [
    "Order list (Content) cannot be empty"
  ]
}