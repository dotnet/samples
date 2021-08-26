# Linux Installation Instructions

In this page, you will find the following information:

* .NET Core version required to run BuggyAmb
* How to run BuggyAmb
  * Running BuggyAmb as a standalone application (no web server is needed)
  * Running BuggyAmb behind Nginx web server

## .NET Core version

BuggyAmb is an ASP.NET Core **framework-dependent** application so it means that the correct version of ASP.NET Core runtime should be installed on your machine.

The main reason for not publishing BuggyAmb as a self-contained application is simple: the size of the package will be much higher than the framework-dependent one when it is deployed as self-contained application because the required .NET Core libraries will also be included in the deployment package. If you want to deploy BuggyAmb as a self-contained application then you can download the source code and publish like that.

Please check the release information to find out which .NET Core version is required to run that release. The initial release of BuggyAmb is an ASP.NET Core 3.1 application so you will need .NET Core 3.1 runtime or SDK. You can run the following command on a terminal to see which versions are installed on your machine:

```dotnet --info```

If you don't have ASP.NET Core 3.1 runtime or SDK on your machine then you can find the installation instructions for different Linux distributions on [this page](/dotnet/core/install/linux).

I have installed the .NET Core 3.1 SDK on Ubuntu 18.04 by following the instructions [here](/dotnet/core/install/linux-ubuntu#1804) and replacing dotnet-sdk-5.0 with dotnet-sdk-3.1:

```
sudo apt-get update;
sudo apt-get install -y apt-transport-https &&
sudo apt-get update &&
sudo apt-get install -y **dotnet-sdk-3.1**
```

## Downloading the BuggyAmb in Linux

Simply you can run the following **wget** command to download BuggyAmb bits on your Linux machine:

```wget https://buggyambfiles.blob.core.windows.net/bin/buggyamb_v1.1.tar.gz```

After the BuggyAmb is downloaded you need to extract the tar.gz file. The file should be downloaded in the current folder you are in when running the **wget** command and now you need to use **tar** to extract the file. I chose extracting the all releases under **/var/buggyamb** directory so I use the following command to extract:

```
sudo mkdir /var/buggyamb
sudo tar -xf buggyamb_v1.1.tar.gz -C /var/buggyamb
```

The **buggyamb_v1.1** folder should have been created under **/var/buggyamb**.

## How to run BuggyAmb

This release of BuggyAmb runs over HTTP. If you need to configure it to run on HTTPS then you can download the source code and make the necessary changes based on your needs.

You need to run BuggyAmb as a standalone application in Linux, there is no "in-process" hosting model for ASP.NET Core applications in Linux unlike what IIS offers when run on Windows.

To run BuggyAmb on Linux:

* Change directory to where BuggyAmb is extracted. In my case I change the directory to **/var/buggyamb/buggyamb_v1.1**
* And run the following command: ```dotnet BuggyAmb.dll```

You should see that the application is listening on port 5000 for HTTP requests.

Now you can test if it works. Open a browser and make a request to BuggyAmb, you should see the BuggyAmb Welcome Page.

Needless to say, "buggyamb" hostname resolves the IP address of my Linux machine and the port 5000 is the port where BuggyAmb listens on.

>This should be enough to get started with troubleshooting. You can start playing around problem scenarios and troubleshoot it. For a quick guide for the problematic scenarios and some troubleshooting tips, you can visit the [quick tour](quick_tour.md).

## Ensuring BuggyAmb runs always

So far, if you are able to access the home page of BuggyAmb, then you are ready to start with troubleshooting. However if you restart the Linux machine or BuggyAmb crashes (and believe it is a buggy application and it crashes a lot), then you should start it manually by running the ```dotnet BuggyAmb.dll``` command. In a real world scenario you want applications to start automatically after a crash or reboot.

If you host an ASP.NET Core application on IIS, either in-process or out-process, IIS manages the process startups. In Linux, you can use **systemd** to manage the same. As described [here](/aspnet/core/host-and-deploy/linux-nginx);

 >systemd is an init system that provides many powerful features for starting, stopping, and managing processes.

**systemd** will use a service/unit file to manage an application. This is similar to the service concept in Windows and is called **daemons** in Linux world. The unit files are located in **/etc/systemd/system** directory.

Here is a sample Unit file that you can use:

```
[Unit]
Description=BuggyAmb ASP.NET Core 3.1

[Service]
WorkingDirectory=/var/buggyamb/buggyamb_v1.1
ExecStart= /usr/bin/dotnet /var/buggyamb/buggyamb_v1.1/BuggyAmb.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=BuggyAmb
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.targe
```

Just create a **buggyamb.service** file in **/etc/systemd/system** directory, copy and paste the lines above in that file. You can use your favorite text editor, such as **nano** or **vi**, e.g.: ```sudo vi /etc/systemd/system/buggyamb.service```

Once you create the unit file, reload the daemon configurations so the system knows about this service:

```sudo systemctl daemon-reload```

Now you are ready to enable the service, start and check if it is running. Enabling a service means that the systemd will be aware of this service so it can be started once the machine is rebooted or the process is crashed. To enable the buggyamb service run this command:

```sudo systemctl enable buggyamb```

Enabling a service does not start it, so you need to start it now - don't worry you won't need to run this command once again unless you explicitly stop the service:

```sudo systemctl start buggyamb```

Now check if the service is started:

```sudo systemctl status buggyamb```

You should see the service is **active (running)**.

The process ID is an important information here because you will need that PID when you trobleshoot BuggyAmb application. You can get the same process ID using other tools like **top** or **htop** easily.

>If you are having problem with running the service, please make sure the unit file is correct. You should update **WorkingDirectory** and **ExecStart** parameters accordingly if you run the BuggyAmb in a directory other than **/var/buggyamb/buggyamb_v1.1**. If those are correct but it still does not correct than you may want to check the "journal" logs by running this command: **sudo journalctl -fu buggyamb.service**

Now BuggyAmb is ready to restart if it crashes or if the machine is rebooted.

## Getting rid of port 5000 - running BuggyAmb behind Nginx

If you are not like "enough, I am done!" yet, there are some other things to do if you want.

Just like the previous "Ensuring BuggyAmb runs always" section, this one is not a requirement to get started. But, if you are like me, you will probably want an environment as close to a real-world scenario as possible, and that port number at the hostname (:5000) will bother you. Why not make requests like `http://buggybits/Problem` instead of `http://buggybits:5000/Problem`, right?

The idea is very simple, Nginx will listen on port 80 and act as a reverse proxy server, and it will send the requests to the BuggyAmb application which listens on port 5000. So, our lovely clients won't have to remember that port number and instead they will just use the hostname. Lovely.

[This article](/aspnet/core/host-and-deploy/linux-nginx) explains how you can configure Nginx as a reverse proxy server.

Let's quickly go through the steps:

### Installing Nginx

Run the following command to install Nginx:

```sudo apt-get install nginx```

After the installation is completed, make sure that the Nginx works correctly. You can run ```sudo nginx -t``` command to check if the Nginx configuration is correct and also can run ```sudo systemctl status nginx``` command to see if the "service" is up and running - you should see **active (running)** output.

> If it is not started you can try ```sudo systemctl start nginx``` or ```sudo service nginx start```. If you are still having trouble installing and running Nginx, please visit the [official Nginx installation page](https://www.nginx.com/resources/wiki/start/topics/tutorials/install/).

### Configuring Nginx to route the requests to BuggyAmb

Nginx is a powerful web server and can be configured to act as a reverse proxy. We can add a "server" block to the configuration file to just tell the Nginx to route the requests to the BuggyAmb application which runs on `http://localhost:5000`. Following sample routes the requests made to `http://buggyamb` to `http://localhost:5000`:

```javascript
server {
    listen        80;
    server_name   buggyamb;
    location / {
        proxy_pass         http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}
```

> Of course the **buggyamb** hostname should resolve to the IP address of the Linux machine on your client machine. You can simply add the **buggyamb** in the client's hosts file, or, if you have a DNS server you can update it there.

  Just open the ```/etc/nginx/sites-available/default``` file and add the server block above. After saving the changes, make sure that the Nginx configuration is correct by running ```sudo nginx -t``` command. You should see "configuration test is successfull" message.

> If you are seeing an error then you probably made a mistake when adding the server block in previous step. Roll back from the configuration backup of Nginx and try again (what? you didn't take a backup before configuration change? You didn't take it because I didn't tell you take it? You are so brave, always take a backup before making an important change - luckily this should not be too difficult to fix :smiley:).

Once the Nginx is configured correctly, let the Nginx to read the configuration changes by running `sudo nginx -s reload` command.

### Testing

You configured Nginx and it is time for a test. First test I'd recommend would be to make a connection test using ```curl``` directly on the Linux server. The goal with this test is to make sure that everything works fine locally.

> curl just makes an HTTP get request to the destination and it just shows the result in plain text. So it should show the HTML output of the response.

 The first thing you need to configure is the hosts file so the **buggyamb** hostname resolves to **127.0.0.1**. Add **buggyamb** in ```/etc/hosts``` file so it resolves to 127.0.0.1. You can use **vi** or **nano** again.

Then run ```curl localhost``` command. Nginx should get the request and show its welcome page because we configured Nginx to route the requests to BuggyAmb if only the hostname is **buggyamb** and this request is not made to **buggyamb**, it is made to **localhost**.

Now run ```curl buggyamb``` command.  This time we make the request with **buggyamb** hostname so if Nginx is configured correctly, it should route the request to the BuggyAmb application running on port 5000. You should see the HTML of BuggyAmb Welcome Page.

> All working? Good. If not working, I'd recommend you to go through the steps above once again, you may be missing something very simple. There are really great articles on the Internet, if you cannot solve it, search for the resolution. Good searcing skills are very useful when troubleshooting problems. If you still cannot find your answer feel free to ask your questions here in the comments, keep in mind that asking questions is one of the necessary steps for troubleshooting. Don't be shy to ask questions.

If everything is working fine, then try to access BuggyAmb from your client machine. If you cannot get the page and instead you get "page cannot be displayed" or a similar error, it may be either a name resolution or a local firewall problem. I would recommend you to make sure **buggyamb** hostname is resolving to the IP address of your Linux machine first. If it is correct then you should configure local firewall to allow HTTP requests coming from remote machines.
