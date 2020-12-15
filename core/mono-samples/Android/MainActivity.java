// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

package net.dot;

import android.app.Activity;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.graphics.Color;

public class MainActivity extends Activity
{
    static
    {
        System.loadLibrary("native-lib");
    }

    // Called from JNI
    public static int getNum()
    {
        return 42;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);

        RelativeLayout rootLayout = new RelativeLayout(this);

        final TextView welcomeMsg = new TextView(this);
        welcomeMsg.setId(1);
        welcomeMsg.setTextSize(20);
        RelativeLayout.LayoutParams welcomeMsgParams =
                                    new RelativeLayout.LayoutParams(
                                        RelativeLayout.LayoutParams.WRAP_CONTENT,
                                        RelativeLayout.LayoutParams.WRAP_CONTENT);
        welcomeMsgParams.addRule(RelativeLayout.CENTER_HORIZONTAL);
        welcomeMsgParams.addRule(RelativeLayout.ALIGN_TOP);
        rootLayout.addView(welcomeMsg, welcomeMsgParams);

        final TextView clickCounter = new TextView(this);
        clickCounter.setId(2);
        clickCounter.setTextSize(20);
        RelativeLayout.LayoutParams clickCounterParams =
                                    new RelativeLayout.LayoutParams(
                                        RelativeLayout.LayoutParams.WRAP_CONTENT,
                                        RelativeLayout.LayoutParams.WRAP_CONTENT);
        clickCounterParams.addRule(RelativeLayout.CENTER_HORIZONTAL);
        clickCounterParams.addRule(RelativeLayout.CENTER_VERTICAL);
        rootLayout.addView(clickCounter, clickCounterParams);

        Button button = new Button(this);
        button.setId(3);
        button.setText("Click me");
        button.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                clickCounter.setText(incrementCounter());
            }
        });
        RelativeLayout.LayoutParams buttonParams =
                                    new RelativeLayout.LayoutParams(
                                        RelativeLayout.LayoutParams.WRAP_CONTENT,
                                        RelativeLayout.LayoutParams.WRAP_CONTENT);
        buttonParams.addRule(RelativeLayout.BELOW, clickCounter.getId());
        buttonParams.addRule(RelativeLayout.CENTER_HORIZONTAL);
        rootLayout.addView(button, buttonParams);

        EditText textField = new EditText(this);
        textField.setId(4);
        textField.setTextSize(20);
        RelativeLayout.LayoutParams textFieldParams =
                                    new RelativeLayout.LayoutParams(
                                        RelativeLayout.LayoutParams.WRAP_CONTENT,
                                        RelativeLayout.LayoutParams.WRAP_CONTENT);
        textFieldParams.addRule(RelativeLayout.CENTER_HORIZONTAL);

        Button enterName = new Button(this);
        enterName.setId(5);
        enterName.setText("Enter");
        enterName.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                welcomeMsg.setText(greetName(textField.getText().toString()));
            }
        });
        RelativeLayout.LayoutParams enterNameParams =
                                    new RelativeLayout.LayoutParams(
                                        RelativeLayout.LayoutParams.WRAP_CONTENT,
                                        RelativeLayout.LayoutParams.WRAP_CONTENT);
        enterNameParams.addRule(RelativeLayout.CENTER_HORIZONTAL);
        enterNameParams.addRule(RelativeLayout.ALIGN_PARENT_BOTTOM);
        rootLayout.addView(enterName, enterNameParams);

        textFieldParams.addRule(RelativeLayout.ABOVE, enterName.getId());
        rootLayout.addView(textField, textFieldParams);

        setContentView(rootLayout);

        final String entryPointLibName = "AndroidSampleApp.dll";
        welcomeMsg.setText("Running " + entryPointLibName + "...");
        clickCounter.setText("Hello World!");
        final Activity ctx = this;
        new Handler(Looper.getMainLooper()).postDelayed(new Runnable() {
            @Override
            public void run() {
                int retcode = MonoRunner.initialize(entryPointLibName, ctx);
                welcomeMsg.setText("Hello Android " + retcode + "!\nRunning on mono runtime\nUsing C#");
            }
        }, 1000);
    }

    public native String incrementCounter();

    public native String greetName(String text);
}
