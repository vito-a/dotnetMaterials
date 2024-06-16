<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/Tasks">
        <html>
        <head>
            <title>Tasks Report</title>
        </head>
        <body>
            <h1>Tasks Report</h1>
            <table border="1">
                <tr>
                    <th>Id</th>
                    <th>Title</th>
                    <th>Description</th>
                    <th>Due Date</th>
                    <th>Completed</th>
                </tr>
                <xsl:for-each select="Task">
                    <tr>
                        <td><xsl:value-of select="Id"/></td>
                        <td><xsl:value-of select="Title"/></td>
                        <td><xsl:value-of select="Description"/></td>
                        <td><xsl:value-of select="DueDate"/></td>
                        <td><xsl:value-of select="IsCompleted"/></td>
                    </tr>
                </xsl:for-each>
            </table>
        </body>
        </html>
    </xsl:template>
</xsl:stylesheet>
