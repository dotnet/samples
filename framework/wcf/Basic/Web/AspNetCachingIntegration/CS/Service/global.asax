<%@ Application Language="C#" %>
<%@ Import Namespace="Microsoft.Samples.AspNetCachingIntegration" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="System.ServiceModel.Activation" %>

<script RunAt="server">
void Application_Start(object sender, EventArgs e)
{
    RegisterRoutes(RouteTable.Routes);
}

private void RegisterRoutes(RouteCollection routes)
{
    routes.Add(new ServiceRoute("Customers", new WebServiceHostFactory(), typeof(Service))); 
}

</script>


