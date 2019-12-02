//
//  InAppPurchaseManager.mm
//

#import "InAppPurchaseManager.h"


char* base64_encode(const void* buf, size_t size)
{
    static const char base64[] =  "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    
    char* str = (char*) malloc((size+3)*4/3 + 1);
    
    char* p = str;
    unsigned char* q = (unsigned char*) buf;
    size_t i = 0;
    
    while(i < size) {
        int c = q[i++];
        c *= 256;
        if (i < size) c += q[i];
        i++;
        
        c *= 256;
        if (i < size) c += q[i];
        i++;
        
        *p++ = base64[(c & 0x00fc0000) >> 18];
        *p++ = base64[(c & 0x0003f000) >> 12];
        
        if (i > size + 1)
            *p++ = '=';
        else
            *p++ = base64[(c & 0x00000fc0) >> 6];
        
        if (i > size)
            *p++ = '=';
        else
            *p++ = base64[c & 0x0000003f];
    }
    
    *p = 0;
    
    return str;
}



@implementation PaymentIOS
@synthesize buy , array;



- (NSString *)encodeBase64:(const uint8_t *)input length:(NSInteger)length
{
    return [NSString stringWithUTF8String:base64_encode(input, (size_t)length)];
}


static PaymentIOS* paymentIOS;
+ ( PaymentIOS* ) instance
{
    if ( !paymentIOS )
    {
        paymentIOS = [ [ PaymentIOS alloc ] init ];
        paymentIOS.array = [ [ NSMutableArray alloc ] init ];
    }
    
    return paymentIOS;
}


- ( NSMutableArray* ) getList
{
    return products;
}


- ( void ) getList:( NSMutableArray* )arr
{
    if ( products )
    {
        return;
    }
    
    if ( [ SKPaymentQueue canMakePayments ] )
    {
        SKProductsRequest* request = [ [ SKProductsRequest alloc ] initWithProductIdentifiers:
                                      [ NSSet setWithArray:arr ] ];
        
        request.delegate = self;
        
        [ request start ];
        
        [ [ SKPaymentQueue defaultQueue ] addTransactionObserver:self ];
    }
    else
    {
        
    }
}


- (void)dealloc
{
    products = NULL;
    
    array = NULL;
}



- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response __OSX_AVAILABLE_STARTING(__MAC_NA,__IPHONE_3_0)
{
    products = [ [ NSMutableArray alloc ] initWithArray:response.products ];
    productsDic = [ [ NSMutableDictionary alloc ] init ];
    
    for( int i = 0 ; i < products.count ; ++i )
    {
        SKProduct* pro = [ products objectAtIndex:i ];
        NSLog( @"%@" , pro.localizedTitle );
        [ productsDic setValue:pro forKey:pro.productIdentifier ];
    }
    
    UnitySendMessage( "GameIOSPayData" , "IAPRequest" , "" );
    
    //[ self buyGoods: 0 ];
    
    //    if ( lastTrans )
    //    {
    //        [ [ SKPaymentQueue defaultQueue ] finishTransaction:lastTrans ];
    //        lastTrans = NULL;
    //    }
}


- (void) completeTransaction:(SKPaymentTransaction *) transaction
{
    NSString *jsonObjectString = [self encodeBase64:(uint8_t *)transaction.transactionReceipt.bytes
                                             length:transaction.transactionReceipt.length];
    
    UnitySendMessage( "GameIOSPayData" , "IAPComplete" , [ jsonObjectString UTF8String ] );
    
    lastTrans = transaction;
    
    //    [ [ SKPaymentQueue defaultQueue ] finishTransaction:transaction ];
}



- (void)failedTransaction:(SKPaymentTransaction *)transaction
{
//   UnitySendMessage( "GameIOSPayData" , "IAPFailed" , [ transaction.error.localizedDescription UTF8String ] );
    
//     if ( transaction.error.code == 0 )
//     {
//         [ [ SKPaymentQueue defaultQueue ] restoreCompletedTransactions ];
//     }
    
    [ [ SKPaymentQueue defaultQueue ] finishTransaction:transaction ];
    lastTrans = NULL;
}


- (void) paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions
{
    for (SKPaymentTransaction *transaction in transactions)
    {
        switch ( transaction.transactionState )
        {
            case SKPaymentTransactionStatePurchased:
                [ self completeTransaction:transaction ];
                break;
            case SKPaymentTransactionStateFailed:
                [ self failedTransaction:transaction ];
                break;
            case SKPaymentTransactionStateRestored:
                [ self failedTransaction:transaction ];
                break;
            case SKPaymentTransactionStatePurchasing:
                break;
            default:
                break;
        }
    }
    
    buy = NO;
    
}


- (void) buyGoods:( int ) index
{
    if ( lastTrans )
    {
        return;
    }
    
    if ( !products || products.count == 0 || products.count < ( index + 1 ) )
    {
        return;
    }
    
    SKProduct *sko = [ products objectAtIndex:index ];
    
    if ( buy )
    {
        return;
    }
    
    buy = YES;
    
    SKMutablePayment *payment = [ SKMutablePayment paymentWithProduct:sko ];
    payment.quantity = 1;
    [ [ SKPaymentQueue defaultQueue ] addPayment:payment ];
}

- ( void ) finishBuyGoods
{
    if ( !lastTrans )
    {
        return;
    }
    
    [ [ SKPaymentQueue defaultQueue ] finishTransaction:lastTrans ];
    lastTrans = NULL;
}

- ( void ) buyGoods1:( NSString* )ia;
{
    if ( !productsDic )
    {
        return;
    }
    
    if ( ![ productsDic objectForKey:ia ] )
    {
        return;
    }
    
    SKProduct *sko = [ productsDic objectForKey:ia ];
    
    if ( buy )
    {
        return;
    }
    
    buy = YES;
    
    SKMutablePayment *payment = [ SKMutablePayment paymentWithProduct:sko ];
    payment.quantity = 1;
    [ [ SKPaymentQueue defaultQueue ] addPayment:payment ];
}

- ( void ) clear
{
    [ array removeAllObjects ];
    array = NULL;
    array = [ [ NSMutableArray alloc ] init ];
    
    products = NULL;
    
    [ [ SKPaymentQueue defaultQueue ] removeTransactionObserver:self ];
}

@end




extern "C"
{
    void iapFinishBuyGoods()
    {
        [ [ PaymentIOS instance ] finishBuyGoods ];
    }
    
    void iapBuyGoods( int n )
    {
        [ [ PaymentIOS instance ] buyGoods:n ];
    }
    
    void iapClearList()
    {
        [ [ PaymentIOS instance ] clear ];
    }
    
    void iapGetList()
    {
        [ [ PaymentIOS instance ].array addObject:@"active0" ];
        [ [ PaymentIOS instance ] getList:[ PaymentIOS instance ].array ];
    }
    
    
}


