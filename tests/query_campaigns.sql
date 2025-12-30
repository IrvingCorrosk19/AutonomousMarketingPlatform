SELECT COUNT(*) as total_campaigns, 
       COUNT(CASE WHEN "Status" = 'Active' THEN 1 END) as active_campaigns 
FROM "Campaigns";

