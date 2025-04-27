# ImageInfoAPI
#Features
Upload multiple images (jpg, png, webp) with max size 2MB.
Auto-convert all images to WebP format.
Auto-resize to 3 dimensions: Phone, Tablet, Desktop.
Extract EXIF metadata (Geo Info, Camera Make & Model) into a .json file.
Return a Unique ID per image for future operations.
Download resized images by ID and size.
Fetch metadata by ID.
All files stored in the file system — no database required.



#Setup Instructions
1. Clone the repository:
 git clone  https://github.com/kareemabdel/ImageInfoAPI.git
 cd ImageInfoAPI
2. Install dependencies:
   dotnet restore
3. Run the project:
   dotnet run

   #Notes
Uploaded original and resized images are saved under /Storage/Originals, /Storage/Resized/Phone, etc.
Metadata files are saved under /Storage/Metadata.
Maximum file size allowed: 2MB per image.


#API contains 
1. Upload Images endpoint
Endpoint: POST /api/images/upload
Request: multipart/form-data
Field: files (multiple images)
Response:
[
  {
    "uniqueId": "c1d9f50f-6d6d-4b09-b708-52d69f32f938"
  }
]

2. Download Resized Image endpoint
 Endpoint: GET /api/images/download
Query Parameters:
uniqueImageId:GUID
size (values: Phone, Tablet, Desktop)
example: GET /api/images/download?uniqueImageId=c1d9f50f-6d6d-4b09-b708-52d69f32f938&size=phone

3.Get Metadata endpoint
Endpoint: GET /api/images/metadata
Query Parameters:
uniqueImageId
GET /api/images/metadata?uniqueImageId=c1d9f50f-6d6d-4b09-b708-52d69f32f938
