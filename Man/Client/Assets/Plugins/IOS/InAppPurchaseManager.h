//
//  InAppPurchaseManager.h
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

#import <StoreKit/SKProductsRequest.h>
#import <StoreKit/SKProduct.h>
#import <StoreKit/SKPaymentQueue.h>
#import <StoreKit/SKPayment.h>
#import <StoreKit/SKPaymentTransaction.h>


@interface PaymentIOS : NSObject< SKProductsRequestDelegate , SKPaymentTransactionObserver >
{
    NSMutableArray* products;
    NSDictionary* productsDic;
    SKPaymentTransaction* lastTrans;
}

@property ( nonatomic ) BOOL buy;
@property ( nonatomic , retain ) NSMutableArray* array;
- ( NSMutableArray* ) getList;
- ( void ) getList:( NSMutableArray* )arr;
- ( void ) buyGoods:( int ) index;
- ( void ) buyGoods1:( NSString* )ia;
- ( void ) finishBuyGoods;

+ ( PaymentIOS* ) instance;

@end


