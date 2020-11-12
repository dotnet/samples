// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#import <UIKit/UIKit.h>
#import "runtime.h"

@interface ViewController : UIViewController
@end

@interface AppDelegate : UIResponder <UIApplicationDelegate>
@property (strong, nonatomic) UIWindow *window;
@property (strong, nonatomic) ViewController *controller;
@end

@implementation AppDelegate
- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    self.window = [[UIWindow alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
    self.controller = [[ViewController alloc] initWithNibName:nil bundle:nil];
    self.window.rootViewController = self.controller;
    [self.window makeKeyAndVisible];
    return YES;
}
@end

UILabel *label;
UILabel *counter;
UITextField *textField;
void (*incrementHandlerPtr)(void);
void (*greetHandlerPtr)(NSString*);

@implementation ViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    
    label = [[UILabel alloc] init];
    [label setTranslatesAutoresizingMaskIntoConstraints:NO];
    label.textColor = [UIColor greenColor];
    label.font = [UIFont boldSystemFontOfSize: 20];
    label.numberOfLines = 3;
    label.textAlignment = NSTextAlignmentCenter;
    label.text = @"Hello iOS!\nRunning on mono runtime\nUsing C#";
    [self.view addSubview:label];
    [label addConstraint:[NSLayoutConstraint constraintWithItem:label
                                             attribute:NSLayoutAttributeWidth
                                             relatedBy:NSLayoutRelationEqual
                                             toItem:nil
                                             attribute:NSLayoutAttributeNotAnAttribute
                                             multiplier:1
                                             constant:300]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:label
                                                 attribute:NSLayoutAttributeCenterX
                                                 relatedBy:NSLayoutRelationEqual
                                                 toItem:self.view
                                                 attribute:NSLayoutAttributeCenterX
                                                 multiplier:1
                                                 constant:0]];                                         

    counter = [[UILabel alloc] init];
    [counter setTranslatesAutoresizingMaskIntoConstraints:NO];
    counter.textColor = [UIColor greenColor];
    counter.font = [UIFont boldSystemFontOfSize: 20];
    counter.numberOfLines = 1;
    counter.textAlignment = NSTextAlignmentCenter;
    counter.text = @"counter";
    [self.view addSubview:counter];
    [counter addConstraint:[NSLayoutConstraint constraintWithItem:counter
                                               attribute:NSLayoutAttributeWidth
                                               relatedBy:NSLayoutRelationEqual
                                               toItem:nil
                                               attribute:NSLayoutAttributeNotAnAttribute
                                               multiplier:1
                                               constant:300]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:counter
                                                 attribute:NSLayoutAttributeTop
                                                 relatedBy:NSLayoutRelationEqual
                                                 toItem:label
                                                 attribute:NSLayoutAttributeBottom
                                                 multiplier:1
                                                 constant:50]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:counter
                                                 attribute:NSLayoutAttributeCenterX
                                                 relatedBy:NSLayoutRelationEqual
                                                 toItem:self.view
                                                 attribute:NSLayoutAttributeCenterX
                                                 multiplier:1
                                                 constant:0]];                                             
    
    UIButton *button = [UIButton buttonWithType:UIButtonTypeInfoDark];
    [button setTranslatesAutoresizingMaskIntoConstraints:NO];
    [button addTarget:self action:@selector(incrementCounter:) forControlEvents:UIControlEventTouchUpInside];
    [button setTitle:@"Click me" forState:UIControlStateNormal];
    [button setExclusiveTouch:YES];
    [self.view addSubview:button];
    [button addConstraint:[NSLayoutConstraint constraintWithItem:button
                                              attribute:NSLayoutAttributeWidth
                                              relatedBy:NSLayoutRelationEqual
                                              toItem:nil
                                              attribute:NSLayoutAttributeNotAnAttribute
                                              multiplier:1
                                              constant:200]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:button
                                                 attribute:NSLayoutAttributeTop
                                                 relatedBy:NSLayoutRelationEqual
                                                 toItem:counter
                                                 attribute:NSLayoutAttributeBottom
                                                 multiplier:1
                                                 constant:10]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:button
                                                 attribute:NSLayoutAttributeCenterX
                                                 relatedBy:NSLayoutRelationEqual
                                                 toItem:counter
                                                 attribute:NSLayoutAttributeCenterX
                                                 multiplier:1
                                                 constant:0]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:button
                                                 attribute:NSLayoutAttributeCenterX
                                                 relatedBy:NSLayoutRelationEqual
                                                 toItem:self.view
                                                 attribute:NSLayoutAttributeCenterX
                                                 multiplier:1
                                                 constant:0]];                                             

    textField = [[UITextField alloc] init];
    [textField setTranslatesAutoresizingMaskIntoConstraints:NO];
    textField.textColor = [UIColor greenColor];
    textField.backgroundColor = [UIColor darkGrayColor];
    textField.font = [UIFont boldSystemFontOfSize: 15];
    textField.textAlignment = NSTextAlignmentCenter;
    textField.placeholder = @"Your name";
    [self.view addSubview:textField];
    [textField addConstraint:[NSLayoutConstraint constraintWithItem:textField
                                                 attribute:NSLayoutAttributeWidth
                                                 relatedBy:NSLayoutRelationEqual
                                                 toItem:nil
                                                 attribute:NSLayoutAttributeNotAnAttribute
                                                 multiplier:1
                                                 constant:125]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:textField
                                                 attribute:NSLayoutAttributeTop
                                                 relatedBy:NSLayoutRelationEqual
                                                 toItem:button
                                                 attribute:NSLayoutAttributeBottom
                                                 multiplier:1
                                                 constant:50]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:textField
                                                 attribute:NSLayoutAttributeCenterX
                                                 relatedBy:NSLayoutRelationEqual
                                                 toItem:self.view
                                                 attribute:NSLayoutAttributeCenterX
                                                 multiplier:1
                                                 constant:0]];

    UIButton *enterName = [UIButton buttonWithType:UIButtonTypeInfoDark];
    [enterName setTranslatesAutoresizingMaskIntoConstraints:NO];
    [enterName addTarget:self action:@selector(greetName:) forControlEvents:UIControlEventTouchUpInside];
    [enterName setTitle:@"Enter" forState:UIControlStateNormal];
    [enterName setExclusiveTouch:YES];
    [self.view addSubview:enterName];
    [enterName addConstraint:[NSLayoutConstraint constraintWithItem:enterName
                                                 attribute:NSLayoutAttributeWidth
                                                 relatedBy:NSLayoutRelationEqual
                                                 toItem:nil
                                                 attribute:NSLayoutAttributeNotAnAttribute
                                                 multiplier:1
                                                 constant:200]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:enterName
                                                 attribute:NSLayoutAttributeTop
                                                 relatedBy:NSLayoutRelationEqual
                                                 toItem:textField
                                                 attribute:NSLayoutAttributeBottom
                                                 multiplier:1
                                                 constant:10]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:enterName
                                                 attribute:NSLayoutAttributeCenterX
                                                 relatedBy:NSLayoutRelationEqual
                                                 toItem:textField
                                                 attribute:NSLayoutAttributeCenterX
                                                 multiplier:1
                                                 constant:0]];

    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
        mono_ios_runtime_init ();
    });
}
-(void) incrementCounter:(UIButton*)sender
{
    if (incrementHandlerPtr)
        incrementHandlerPtr();
}
-(void) greetName:(UIButton*)sender
{
    if (greetHandlerPtr)
        greetHandlerPtr(strdup ([textField.text UTF8String]));
}

@end

// called from C# sample
void
ios_register_counter_increment (void* ptr)
{
    incrementHandlerPtr = ptr;
}

// called from C# sample
void
ios_set_text (const char* value)
{
    NSString* nsstr = [NSString stringWithUTF8String:strdup(value)];
    dispatch_async(dispatch_get_main_queue(), ^{
        counter.text = nsstr;
    });
}

// called from C# sample
void
ios_register_name_greet (void* ptr)
{
    greetHandlerPtr = ptr;
}

// called from C# sample
void
ios_greet_name (const char* value)
{
    NSString* nsstr = [NSString stringWithUTF8String:strdup(value)];
    dispatch_async(dispatch_get_main_queue(), ^{
        label.text = [NSString stringWithFormat:@"%@%@%@", @"Hello ", nsstr, @"!\nRunning on mono runtime\nUsing C#"];
    });
}

int main(int argc, char * argv[]) {
    @autoreleasepool {
        return UIApplicationMain(argc, argv, nil, NSStringFromClass([AppDelegate class]));
    }
}
