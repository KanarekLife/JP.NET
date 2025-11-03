module Lab06.Library.Types.ContactInfo

type ContactInfo =
    | EmailOnly of EmailAddress.T
    | PostOnly of string
    | EmailAndPost of EmailAddress.T * string