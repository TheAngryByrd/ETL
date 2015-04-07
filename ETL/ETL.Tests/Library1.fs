namespace ETL.Tests

module Tests =

    open FsCheck
    open Xunit

    [<Fact>]
    let ``Reverse of reverse of a list is the original list``() =
      let revRevIsOrig (xs:list<int>) = List.rev(List.rev xs) = xs
      Check.QuickThrowOnFailure revRevIsOrig 
