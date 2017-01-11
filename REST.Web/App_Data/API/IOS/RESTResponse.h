#import <Foundation/Foundation.h>

@interface RESTResponse : NSObject
{
	/**
	 * 响应编码
	 */
	@public
    NSString * ErrorCode;

    /**
     * 响应描述
     */
	@public
	NSString * ErrorMsg;

    /**
     * 响应体
     */
	@public
	NSString * Body;
}
@end