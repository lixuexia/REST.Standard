#import <Foundation/Foundation.h>

@interface RESTResponse : NSObject
{
	/**
	 * ��Ӧ����
	 */
	@public
    NSString * ErrorCode;

    /**
     * ��Ӧ����
     */
	@public
	NSString * ErrorMsg;

    /**
     * ��Ӧ��
     */
	@public
	NSString * Body;
}
@end