// Module containing functions related to String50 type
module String50

    type T = String50 of string
        let create (s:string) =
            if s <> null && s.Length <= 50
            then Some (String50 s)
            else None