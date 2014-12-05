workestra-net-sdk
==========

A .NET SDK library for the [Workestra API](https://www.workestra.co/developers/docs)


Quick Start
===========
You will need an API key to get started. You can find instructions on obtaining an API key [here](https://www.workestra.co/developers/docs#authentication)

Once you have that, the following code will get the latest notifications (as long as your user is able to access those notifications)

````

Module Test
    Public Sub Main()

        ' Your user's api key here
        Dim cl As BambooAPIClient = New BambooAPIClient(companySubdomain)
        cl.setApiKey("{Your-API-Key}")

        ' Or your Username and password
        cl.setApiKey("{Email}")
        cl.setApiKey("{Password")

        Dim resp As WorkestraHTTPResponse


        resp = cl.listNotifications()
        System.Console.WriteLine(resp.getContentString())

    End Sub

End Module


````
After that, you may want to explore the stream [api](https://www.workestra.co/developers/docs#sream), or just look through the wrapper code.
