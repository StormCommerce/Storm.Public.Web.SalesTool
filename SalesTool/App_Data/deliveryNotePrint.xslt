﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:n="http://schemas.enferno.se" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="html" />
<xsl:template match="/n:OrderModel">
  <html>
    <body>
      <table align="center" border="0" cellpadding="0" cellspacing="0" style="width:580px">
        <tbody>
          <tr>
            <td style="padding-top:20px" valign="top">
              <!-- Start Header-->
              <center>
                <a href="http://www.cervera.se">
                  <img alt="Cervera" border="0" src="http://cervera.se/Static/img/logo.png" style="max-width: 200px;" />
                </a>
              </center>
              <!-- End Header -->

              <table align="center" border="0" cellpadding="0" cellspacing="0" style="margin:0 auto; width:100%">
                <tbody>
                  <tr>
                    <td style="font-size: 16px; color: #272727; font-weight: normal; text-align: left; font-family: Arial, sans-serif; line-height: 24px; vertical-align: top; padding:10px 8px 10px 8px">
                      <h1 style="text-decoration: none; color: #272727; font-size: 24px; color: #272727; font-weight: normal; font-family:Arial, sans-serif ">Utl&#228;mningsorder</h1>

                      <div>
                        Du har best&#228;llt dessa varor på cervera.se f&#246;r uth&#228;mtning i butik.
                      </div>
                    </td>
                  </tr>
                  <tr>
                    <td style="font-size: 16px; color: #272727; font-weight: normal; text-align: left; font-family: Arial, sans-serif; line-height: 24px; vertical-align: top; padding:10px 8px 10px 8px">
                      <h3 style="margin-bottom: 5px;">Din best&#228;llning</h3>
                      <strong>Ordernummer <xsl:value-of select="n:OrderNumber"/></strong>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <table border="0" cellpadding="0" cellspacing="0" style="font-size: 16px; color: #272727; font-weight: normal; text-align: left; font-family: Arial, sans-serif; line-height: 24px; vertical-align: top; padding:10px 8px 10px 8px; width:100%">
                        <thead>
                          <tr>
                            <td style="padding: 5px 0; border-top: 1px solid #8a8a8a; font-weight: bold;">Best&#228;llare</td>
                            <td style="padding: 5px 0; border-top: 1px solid #8a8a8a; font-weight: bold;">Uth&#228;mtas i butik</td>
                          </tr>
                        </thead>
                        <tbody>
                          <tr>
                            <td style="vertical-align: top;">
                              <xsl:value-of select="n:Buyer/n:Name"/><br />
                              <xsl:value-of select="n:Buyer/n:Email"/>
                            </td>
                            <td style="vertical-align: top;">
                              <strong>
                                <xsl:value-of select="n:PickupStore/n:Name" />
                              </strong>
                              <br />
                              <xsl:value-of select="n:PickupStore/n:Address/n:Address1" />
                              <br />
                              <xsl:value-of select="n:PickupStore/n:Address/n:Zip" />
                              <xsl:value-of select="n:PickupStore/n:Address/n:City" />
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </td>
                  </tr>
                  <tr>
                    <td style="padding: 10px;">
                      <table border="0" cellpadding="0" cellspacing="0" style="margin:0 auto; font-family: Arial, sans-serif; width:100%">
                        <thead>
                          <tr>
                            <td style="border-bottom: 1px solid #8a8a8a;  padding: 5px 0; width:50%">Beskrivning</td>
                            <td style="border-bottom: 1px solid #8a8a8a; padding: 5px 0; text-align: right; width:25%">Antal</td>
                            <td style="border-bottom: 1px solid #8a8a8a; padding: 5px 0; text-align: right; width:25%">Pris</td>
                          </tr>
                        </thead>
                        <tbody>
                          <xsl:for-each select="n:Rows/n:OrderRowModel">
                          <tr>
                            <td style="border-bottom: 1px solid #8a8a8a; padding: 5px 0;">
                              <xsl:value-of select="n:Name"/>
                            </td>
                            <td style="border-bottom: 1px solid #8a8a8a;  padding: 5px 0; text-align: right;">
                              <xsl:value-of select="format-number(n:Quantity, '0')"/>
                            </td>
                            <td style="border-bottom: 1px solid #8a8a8a;  padding: 5px 0; text-align: right;">
                              <xsl:value-of select="format-number(n:PriceInclVat, '0')"/>&#160;kr
                          </td>
                          </tr>
                          </xsl:for-each>
                          <tr>
                            <td colspan="3" style="text-align: right; padding: 10px 0; text-align: right;">
                              Summa: <strong>
                                <xsl:value-of select="format-number(n:TotalInclVat, '0')"/>&#160;kr</strong>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </td>
                  </tr>
                  <tr>
                    <td style="font-size: 16px; color: #272727; font-weight: normal; text-align: left; font-family: Arial, sans-serif; line-height: 24px; vertical-align: top; padding:10px 8px 10px 8px">Jag bekr&#228;ftar att jag har h&#228;mtat ut ovanst&#229;ende varor fr&#229;n butiken.</td>
                  </tr>
                  <tr>
                    <td style="font-size: 16px; color: #272727; font-weight: normal; text-align: left; font-family: Arial, sans-serif; line-height: 24px; vertical-align: top; padding:10px 8px 10px 8px">_______________________________________________<br>
                      <small>Kundens signatur</small>
                    </br>
                  </td>
                  </tr>
                  <tr>
                    <td style="padding: 10px; text-align: center;">
                      <table align="center" border="0" cellpadding="0" cellspacing="0" style="margin:0 auto; width:100%">
                        <tbody>
                          <tr>
                            <td style="font-size: 16px; color: #272727; font-weight: normal; text-align: left; font-family: Arial, sans-serif; line-height: 24px; vertical-align: top; padding: 10px 10px 10px 0 ;">Alltid hos Cervera:</td>
                            <td style="font-size: 16px; color: #272727; font-weight: normal; text-align: left; font-family: Arial, sans-serif; line-height: 24px; vertical-align: top; background: url(http://cervera.se/static/img/check.png) no-repeat 0 5px; padding: 10px 10px 10px 30px;">Fri frakt till butik</td>
                            <td style="font-size: 16px; color: #272727; font-weight: normal; text-align: left; font-family: Arial, sans-serif; line-height: 24px; vertical-align: top; background: url(http://cervera.se/static/img/check.png) no-repeat 0 5px; padding: 10px 10px 10px 30px;">Snabb leverans</td>
                          </tr>
                          <tr>
                            <td>&#160;</td>
                            <td style="font-size: 16px; color: #272727; font-weight: normal; text-align: left; font-family: Arial, sans-serif; line-height: 24px; vertical-align: top; background: url(http://cervera.se/static/img/check.png) no-repeat 0 5px; padding: 10px 10px 10px 30px;">Fri retur i butik</td>
                            <td style="font-size: 16px; color: #272727; font-weight: normal; text-align: left; font-family: Arial, sans-serif; line-height: 24px; vertical-align: top; background: url(http://cervera.se/static/img/check.png) no-repeat 0 5px; padding: 10px 10px 10px 30px;">14 dagar &#246;ppet k&#246;p</td>
                          </tr>
                        </tbody>
                      </table>
                    </td>
                  </tr>
                </tbody>
              </table>

              <div style="height:15px;margin:0 auto;">&#160;</div>

              <table align="center" border="0" cellpadding="0" cellspacing="0" style="width:100%">
                <tbody>
                  <tr>
                    <td style="padding: 0">
                      <table align="center" border="0" cellpadding="0" cellspacing="0" style="margin:0 auto; width:100%">
                        <tbody>
                          <tr>
                            <td>
                              <table align="left" border="0" cellpadding="0" cellspacing="0" style="width:100%">
                                <tbody>
                                  <tr>
                                    <td class="center" style="font-size: 12px; color:#5186bf; font-family: Arial, sans-serif; padding-bottom:20px" valign="top">
                                      <h2 style="font-size: 16px; font-family: Arial, sans-serif; font-weight: normal; margin: 0 0 5px;">KUNDTJ&#196;NST E-HANDEL</h2>
                                      08-630 94 70<br />
                                      ehandel@cervera.se
                                      <div style="color: #5186bf; font-size: 18px; margin-top: 5px;">M&#229;n-fre 10-16, Lunchst&#228;ngt 12-13</div>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </td>
                  </tr>
                </tbody>
              </table>
            </td>
          </tr>
        </tbody>
      </table>
    </body>
  </html>
  </xsl:template>
</xsl:stylesheet>