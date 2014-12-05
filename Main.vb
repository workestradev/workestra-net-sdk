
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
