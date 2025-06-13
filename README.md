# car-rental-cw

## Required SQL function
```sql
create function get_car_list(userid text)
    returns TABLE(id uuid, offerid uuid, normalrate numeric, ratemodifier numeric, discountedrate numeric)
    language plpgsql
as
$$
DECLARE
    eligible BOOLEAN;
    offers "Offers";
    minReqCount numeric;
    offerId uuid;
    rate numeric(10,2);
BEGIN
    select "Value"::numeric from "Configs"
    where "Code"='DB'
    and "Key"='MIN_OFFER_REQ_COUNT'
    into minReqCount;


    SELECT COUNT(*) >= minReqCount INTO eligible
    FROM "Rents"
    WHERE "RequestedById" = userId
    GROUP BY "RequestedById";

    RAISE NOTICE 'Min: %', minReqCount;
    RAISE NOTICE 'first round: %', eligible;


    select * from "Offers" o
    where CURRENT_DATE between o."StartDate" and o."EndDate"
    order by "CreatedOn" desc
    limit 1
    into offers;

    IF (eligible or offers."Type"=1)  THEN
        eligible := true;
        IF offers."Id" is null then
            rate := (100-10.0)/100;
        else
            rate := (100.0 - offers."Discount") / 100;
        end if;
    ELSE
        if offers."Type" = 1 then
            eligible := true;
            rate := (100.0 - offers."Discount") / 100;
        else
            eligible := false;
            rate := 1;
        end if;
    END IF;

    RateModifier := 100 - (rate * 100);

    RAISE NOTICE 'Offers: %', offers."Id";
    RAISE NOTICE 'OFFER COUNT: %', count(offers);
    RAISE NOTICE 'discountRate: %', rate;
    Raise NOTICE 'eligibility: %', eligible;

    if eligible then
        offerid = offers."Id";
    end if;

    RETURN QUERY
    SELECT c."Id", offerid , c."Rate", RateModifier, s1."DiscountedRate"
    FROM "Cars" c
    JOIN (
        SELECT c1."Id", (c1."Rate" * rate) "DiscountedRate"
        FROM "Cars" c1
    ) s1 ON c."Id" = s1."Id"
    where c."ActiveStatus" = true;
END;
$$;

```
