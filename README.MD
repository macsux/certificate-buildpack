This buildpack offers ability to load custom certificates into .NET X509Store in Cloud Foundry in both Windows and Linux stacks. Certificates are installed into `CurrentUser/My` store.

Supported ways to load certificates:

### Environmental Variables

Set `CERTIFICATES__CURRENTUSER__MY__#__CERTIFICATE` environmental variable(s). Replace # with an incrementing number starting at 0 for each certificate installed. If certificate key is encrypted, set matching `CERTIFICATES__CURRENTUSER__MY__#__PASSWORD` variable.

### User Provided Service

Create a user provided service and bind it to your services, with keys `certificate` and optionally `password` if certificate is encrypted.

### Credhub Service Broker

Create and bind credhub service instance with same settings as for User Provided Service.

### Spring Cloud Config Server

Create YAML file with name of your app and load certs under configuration key for your app under `Certificates:CurrentUser:My`. Set `SPRING__APPLICATION__NAME` environmental variable to map app the the YAML configuration file. Example configuration:

```yaml
Certificates:
    CurrentUser:
        My:
        - Certificate: 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
        - Certificate: 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
          Password: hi
        - Certificate : |
            -----BEGIN CERTIFICATE-----
            MIICpDCCAYygAwIBAgIIaSvqYe1ctq4wDQYJKoZIhvcNAQELBQAwEjEQMA4GA1UE
            AwwHcGVtLXB3ZDAeFw0xOTExMTkwMDAwMDBaFw0yMTExMTkwMDAwMDBaMBIxEDAO
            BgNVBAMMB3BlbS1wd2QwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQC3
            cRNNuMPFbJg+sS2b09koxu/4CjaDkP5QFeyqoAbgrnvUpkZhnBIMOFu0oiLBoUV9
            7JA96mSAV6Sa60QU4Ibqyvd7KNbvFuib+i17iws+CJ1K6d8x9vXlNNFP9Q6fEd6c
            vYV+p/rD0cSsAVxmXV9PzgSAjczWMiOuBmS06G5hMG3ZbOsKF2qSaDQRZ9K+1jmo
            DpHuvKcjmaZ1VB/o+S1lTs5DoUEU3mido5zq/oJMOXfIpM6xDSU6xq9mdTYj8qCp
            NKzdXN6zX5eKP4vQ1nAiEcKTSz0pnQZJBmcb3kxOM3BmNWPZP1H6D9b6WPbYPpco
            hwGTpllaLeCe4Lu5jcHPAgMBAAEwDQYJKoZIhvcNAQELBQADggEBACEcyNLvrIoi
            4tndIyJ9vztpRPitywj6I4XYr3SmFKxLR7PG0XKO3vXM4ogvyHc09ZtUehtgt5fP
            Qj0El3wRvbgjn95PRKpYicG3+mKIjTtvyfvI7P55E+Dl+tpAb1MAS+a5XxM51T6+
            WYoKQquI5AFzn/Mroi3E/TWbQTtSC8/1ppN7kMAPS2OjcWzC4ZabwhOUMwhLFxGb
            TpZPn0FQjm2ndSzwA7XnREG5RMMbpx8owUP9+EOQncvlRN8fx4vch//dX9KWKvcG
            cEMrpGObkUIOZsOnb7Vxtl8xubv0Zwm6j+7X0UtiRS9Wfs2TWuEXP+rprFHW6w48
            LmysIM1gpq0=
            -----END CERTIFICATE-----
            -----BEGIN RSA PRIVATE KEY-----
            Proc-Type: 4,ENCRYPTED
            DEK-Info: DES-EDE3-CBC,05dd450f9d67d49f

            MI7hePRYYBLO9slxgzbPcaTEE4tqwdoYqjl0dE+c5IBxgbocR35R7QD9s7G5Xd9a
            /MbC1v816kroPMdGsPWrKMpHbb1aY/v0Wu4gxgeSxBf/p70zWJCjPONO6mYxV6kX
            iy1LJ5fPlNrYUMldeE8kT9NIHftn/Ngzj/Gk1A9x8seICcUeo0nz4R7zPqBG2Qa3
            s6FTAUSmjg1Lh+bhDTr3IN9RZBX4V5nEtegP2CTmpCCre2umu85cdNAcTE2Biy83
            1garZ7RksGj1hEm9RfqVP3O5GwO8CpZ43jHrmhpxqZdQxRBHy/H0pD7BZ7Re/A72
            LahuAFsy1egBbXKVAqg4a5Y8pDwQYnfFgpvDnzVVGoKkK3svLPcOzBRQP45/W0ry
            zoHNj84aAkqqemFcr+etlwTelSohhnRfcED7KeXP6alNFwsNPjAzMIf6Nhb9Mgcl
            7pYF60wygtQkOS5PBxzPRTkBIzpJ6V2CixvEl8joBopKIRLQ9EFED7YmARp8OW1w
            IZ5jpU4O/Y8bguhvxEG9bCBOQEaUtWwbTPy+sFa+MJmItJwwtK00qtzVa4+Q5j/h
            7rk81suCLRUazXQP8CBDiuexWHiiGS/eacoTNksiD/jeumpfQTKOPUW7JleMOlhW
            z3uJNV0BNtEe3uH9dujCH1V9WEYBy+S3h5uBJAXsv6aM2SV4sH5kXRwEsM0P7XPj
            S3VNrn/73s4Kq8lnE0ox/sBkLCNSLgwBWIsJ9FijG5KPXrTvfvr3f2HFj7bgJ/Hy
            T50LtcfUAsa+7V3vIdD+3tNEymBSXl8JC/5CzaZZpvulJbPgzRf4jiXF6FR6fh1E
            LBmhdn73wdtxZy56iO1gfEIG34QPPyHWHDHI01eLduolSKHwo0g546wnSssWGKGp
            jAhBRocbPwNyaEmtb1AWlzBG4bzWBv33cHqz8h9MrnWpN5wnhv67fyVgT6R4ssbV
            CHRc/tp6orbWc+SelKcdfWydUuxexkkno6yE84LteJY7WzUiidRIR/ilH8tpVc8l
            kb787UPBFieY5P6pGjoxNA19RNxvuv4DszdVlBT7hiHs9EGjq7gkYAHvoM8is9jU
            7gdlvfpVal3wOM4Xk4hrWKOGERaQFJnavMonHs0++rl616XPtdFPrDZ8DBkgxENo
            ouM7pLRMFarEFfRoEL/MCljwyII1zVH6XIDxEsiSqztZ2G8UFlJTTq2JYuac3GQ6
            TJbHxNRLfUo6K8W/vKGVi6n25ZXqW4L7q3c5aye7au3HltulCdgDWAXY2Xe9V6BH
            YmFjM4rwJt0ivZkIzB+QZ3kYUi+i/SFtGP9KJG1OKkRTBVecHGcx34x9nosdLZP/
            6i/E3aAN2DTBMiFucmyqrdAaGKHYmnuUNUHVMUmXBHHlSnFnl/9Tu89jFanSOLdm
            92KpAmCPwm5uPmeqhsiU8zIy54AMSgTQS0EqrWASILZOXrPfdOTQnaI3p9QtX0Nn
            +J5w8mifgY5qIHLgS2US5ncdYtY2AZpWDEmWxhF6xHMJYJ01tzBFNnTdMaRJiVNx
            Np1I5x1o9daxqERi9oEO8c6vmxV/vnAHnCTcQ/ai7J51qhnN2miE1CWPYKGPmBeh
            -----END RSA PRIVATE KEY-----
          Password: hi
        - Certificate: |
            -----BEGIN CERTIFICATE-----
            MIICqjCCAZKgAwIBAgIIDZsk1mIXJUswDQYJKoZIhvcNAQELBQAwFTETMBEGA1UE
            AwwKcGVtLW5vLXB3ZDAeFw0xOTExMTkwMDAwMDBaFw0yMTExMTkwMDAwMDBaMBUx
            EzARBgNVBAMMCnBlbS1uby1wd2QwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEK
            AoIBAQCHEMFecWX1jJqmQOPHprCYmg6cQYKNelazMCRIJ+Bcw8UPbrbU8/AoYUBz
            EbZ0Ordq0XmjVLeSg6XlvHsyfQ1jW7mecaV3BCA095wM0EXaeqt+fP7yJkTwB1As
            vr+NjvOzDIvNYGD/3/NMdTP+RmfWrYvZE9+kRSlhIkPW850CreILoJA95UyUJ2zJ
            /M5VDFqkMUth8rhVtMrtJKZ1KEqIU8gG5mpK6omDcvy8uonthZCE8Gwc2wD2WFkr
            meUaH9prgqMlk8ZHVBvuCB0RK0ooICr2Xh5q85Z3E+n1U+WEuE1UwZnsYtfta7Ni
            JRjKftP+WcNQP7HfIZw0O45Wdkx/AgMBAAEwDQYJKoZIhvcNAQELBQADggEBAAhP
            3+ygsXMGvGudoQLIm+0n/aLB6KWSUtBYS4MJ8o9Rnk7EPoUXTq6HbS8q/ECyIQlD
            nbHoYK+jNgYizxbuR7YC5a2M0xnP4g5b4EE/dtAPLd+jvjE2tTErbvEqxPDnk8KB
            20vYEnGqfUuVesqINreIu6q7JG2+KBIq7pLPklMsTYiSNZ+bzeroon9WP6MyQ2VS
            CXaB7nM/QRwOH8Gs3AsVr43awtL/c7nzId1Mzv0sHKj7o7WnaFpJdZQxfzhGQsWL
            vOq3oo2pQG867V8U5IYvUnqiqvT6Ry/Yfzqgg6fOcYZ7hOoz6WrIUbgg6paadRNM
            Za0WLWZyHyYxORYPPVY=
            -----END CERTIFICATE-----
            -----BEGIN RSA PRIVATE KEY-----
            MIIEogIBAAKCAQEAhxDBXnFl9YyapkDjx6awmJoOnEGCjXpWszAkSCfgXMPFD262
            1PPwKGFAcxG2dDq3atF5o1S3koOl5bx7Mn0NY1u5nnGldwQgNPecDNBF2nqrfnz+
            8iZE8AdQLL6/jY7zswyLzWBg/9/zTHUz/kZn1q2L2RPfpEUpYSJD1vOdAq3iC6CQ
            PeVMlCdsyfzOVQxapDFLYfK4VbTK7SSmdShKiFPIBuZqSuqJg3L8vLqJ7YWQhPBs
            HNsA9lhZK5nlGh/aa4KjJZPGR1Qb7ggdEStKKCAq9l4eavOWdxPp9VPlhLhNVMGZ
            7GLX7WuzYiUYyn7T/lnDUD+x3yGcNDuOVnZMfwIDAQABAoIBABbr3wUaFYWQcQZZ
            6M4PGdUr/+LAs1kzM9eBCgc8zTSw/JZb4adOJqmTp45fqSkcddgo6I2xaN3yfYr5
            a6br1gPQozWib8Q87QJQKyl5hwFG5Ce5KsMJf5WEiDSvwm5RnkX6kRcvjgaeiPmm
            wkYayLCED0Gtv7xvCFC1sfGesjyr6MVCB53vd0Fl6AsG2GA0PsjfbGbdCL69bizY
            sZgqkxC5aNtCQE+5rlEMEr+uohGhJWnlODGqFGpjmY+rZhWMN8DfqBYgODzCDPMy
            nK8ZBaj8xlIF7VeCcPsA30e0w5O+jhmhhnYKG/iAIcAMD8GnvHr/pWFU1nKtOgZA
            FcZ8sBECgYEA7KvO39IaJjz/gL708YKfMGbPAyAbEZxXbItjTgs/gs8rYpo1NWEO
            1ph6cQQBPsQ+4RtjXi/N5RyfGENEWDrONLrA8uXO0l+RsTXrhUZbHTJzrbcNG4kK
            T0CJKsFVZ8lLKCJZ4y4TY1LbyKmMSDb9YbCu1eMkgaxMHglNVHAwkEUCgYEAkhih
            eHGF4smSaeGIZ96E8TkNRcPEDKaITFRuYV7nL8c7yVnWpJg9D8yrw6jbIo1riP7I
            Ea41ELMCCS0WlInTQGN6RCoIqWX7AB6kNuU+dTAskwDjL8eJymEU5BwZ2jRbYSFJ
            7VnNl0EFn0/Ta/NQ4GbkUwCwZHWXj04BssYhH/MCgYAUyj7hAHbVlxQmvh+MO5o6
            ST20G5TlJiOz4Ea361sH5aT9A2jbKIyCI2DWHrDxT5HqelpvQqnGj44WNoqBaaEF
            lsYuUCur5UNKGt92lKiaVv08bFEVcyNZtOSXweX7uP4GJ4ZX44tGdVfjN6mgClyV
            wUyKc2qYjYceOFj3qudJNQKBgAeo2u5hpU9amh6LAIfRls6+2v9JYEH5iTPXSWI5
            g41xuKS53RMMGEd3x5mehL6vA3BPS1YB+C5D9FtezcxD3ZuGm6ZvPb0D58D92kY2
            hqvGNtAOP9sFbyf5Jws+nexeyluxTxrulmTTt+O+X6tNOnRJcLcZViYuNCADSvuA
            f8QZAoGADohCE4C9VNwhjWjlRQl8UVbIsTlBbao94QFsMUvZF8NK6zwuk5Cekstr
            JNopbeIdn2cDpRe3uq02fKImOl+4KNepK3NSFzM2zo8/+Y0aQ9PD6Nw6e4C8fguA
            kpaegoUWbB6VMOjeY6R7TnTvkUO8MvBYHH+HLinoC4axJIQ37Kc=
            -----END RSA PRIVATE KEY-----

```



## Certificate Formats

You can use PFX (aka Pkcs12) or PEM based formats. If using PFX, bytes must be Base64 encoded. PEM format is already Base64 encode and must include both certificate and private key blocks. See examples above. If private key is encoded, Password field must be set.

### Notes

If generating self signed certificates (especially using Bouncy Castle), set password to empty string instead of null if you don't want to use passcode, otherwise OpenSSL provider will fail to load it when running on Linux stack.



## FAQ

Q: How can I populate other credentials stores

A: Not supported due to security constraints of the container 

Q: I need to install company's Root CA for my app

A: Work with your platform team to do this through Pivotal Operations Manager (or BOSH manifest if running open source). [More info](https://docs.pivotal.io/pivotalcf/2-5/security/pcf-infrastructure/certificates-index.html)