#import <Foundation/Foundation.h>

@interface WebUtils : NSObject
{
	@property int TimeOut;
}
+ (NSString *)SignRequest:(Map<String, String>)parameters:(NSString *)secret;
+ (NSString *)Md5:(NSString *)str;
+ (NSString *)RESTPost:(NSString *)url:(Map<String, String>)postParameters;
@end