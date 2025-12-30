SELECT COUNT(*) as total_content, 
       COUNT(CASE WHEN "ContentType" = 'Image' THEN 1 END) as images, 
       COUNT(CASE WHEN "ContentType" = 'Video' THEN 1 END) as videos 
FROM "Contents";

