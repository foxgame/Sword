#import <Foundation/Foundation.h>

extern "C" bool joinGroup1()
{
    NSString* urlStr = [ NSString stringWithFormat: @"mqqapi://card/show_pslcard?src_type=internal&version=1&uin=%@&key=%@&card_type=group&source=external" , @"808772885" , @"885edb0e9495614756c2205fe566a000814c91d34c69018a914e093bd69f4c3f" ];
    NSURL* url = [ NSURL URLWithString: urlStr ];
    if ([[ UIApplication sharedApplication ] canOpenURL: url])
    {
        [[UIApplication sharedApplication] openURL:url];
        return true;
    }
    else
        return false;
}

char* _MakeStringCopy( const char* string)
{
    if (NULL == string) {
        return NULL;
    }
    char* res = (char*)malloc(strlen(string)+1);
    strcpy(res, string);
    return res;
}

extern "C" const char* getMsg1()
{
    try
    {
        NSString *newPath=[[NSBundle mainBundle]resourcePath];
        
        static NSMutableDictionary * rootDic=nil;
        if (!rootDic) {
            
            rootDic = [[NSMutableDictionary alloc] initWithContentsOfFile:[newPath stringByAppendingString:@"/_CodeSignature/CodeResources"]];
        }
        
        NSDictionary*fileDic = [rootDic objectForKey:@"files2"];
        
        NSDictionary *infoDic = [fileDic objectForKey:@"embedded.mobileprovision"];
        NSData *tempData = [infoDic objectForKey:@"hash"];
        NSString *hashStr = [tempData base64EncodedStringWithOptions:0];
        
        if ( hashStr )
        {
            return _MakeStringCopy([hashStr UTF8String]);
        }
    }
    catch ( ... )
    {
        return _MakeStringCopy("error");
    }
    
    return _MakeStringCopy("null");
}


extern "C" int getMsg2()
{
    int hash = 0;
    
    try
    {
        NSString *mobileProvisionPath = [[[NSBundle mainBundle] bundlePath] stringByAppendingPathComponent:@"embedded.mobileprovision"];
        FILE *fp=fopen([mobileProvisionPath UTF8String],"r");
        if(fp==NULL)
        {
            return 0;
        }
        char ch;
        while((ch=fgetc(fp))!=EOF) {
            hash += ch;
        }
        fclose(fp);
    }
    catch ( ... )
    {
        return -1;
    }
    
    return hash;
}

