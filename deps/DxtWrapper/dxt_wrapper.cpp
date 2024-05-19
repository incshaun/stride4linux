// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
#include "dxt_wrapper.h"

#include <iostream>

// Utilities functions
void dxtComputePitch( DXGI_FORMAT fmt, int width, int height, int& rowPitch, int& slicePitch, int flags = DirectX::CP_FLAGS_NONE )
{
    std::cout << "DX A19\n";
	size_t rowPitchT, slicePitchT;
	DirectX::ComputePitch(fmt, width, height, rowPitchT, slicePitchT, (DirectX::CP_FLAGS) flags);
	rowPitch = rowPitchT;
	slicePitch = slicePitchT;
}

bool dxtIsCompressed(DXGI_FORMAT fmt) { return DirectX::IsCompressed(fmt); }

HRESULT dxtConvert( const DirectX::Image& srcImage, DXGI_FORMAT format, int filter, float threshold, DirectX::ScratchImage& cImage )
{
    std::cout << "DX A18\n";
	return DirectX::Convert(srcImage, format, (DirectX::TEX_FILTER_FLAGS) filter, threshold, cImage);
}

HRESULT dxtConvertArray( const DirectX::Image* srcImages, int nimages, const DirectX::TexMetadata& metadata, DXGI_FORMAT format, int filter, float threshold, DirectX::ScratchImage& cImage )
{
    std::cout << "DX A17\n";
	return DirectX::Convert(srcImages, nimages, metadata, format, (DirectX::TEX_FILTER_FLAGS) filter, threshold, cImage);
}

HRESULT dxtCompress( const DirectX::Image& srcImage, DXGI_FORMAT format, int compress, float alphaRef, DirectX::ScratchImage& cImage )
{
    std::cout << "DX A16\n";
	return DirectX::Compress(srcImage, format, (DirectX::TEX_COMPRESS_FLAGS) compress, alphaRef, cImage);
}

HRESULT dxtCompressArray( const DirectX::Image* srcImages, int nimages, const DirectX::TexMetadata& metadata, DXGI_FORMAT format, int compress, float alphaRef, DirectX::ScratchImage& cImages )
{
    std::cout << "DX A15\n";
	return DirectX::Compress(srcImages, nimages, metadata, format, (DirectX::TEX_COMPRESS_FLAGS) compress, alphaRef, cImages);
}

HRESULT dxtDecompress( const DirectX::Image& cImage, DXGI_FORMAT format, DirectX::ScratchImage& image )
{
    std::cout << "DX A14\n";
	return DirectX::Decompress(cImage, format, image);
}

HRESULT dxtDecompressArray( const DirectX::Image* cImages, int nimages, const DirectX::TexMetadata& metadata, DXGI_FORMAT format, DirectX::ScratchImage& images )
{
    std::cout << "DX A13\n";
	return DirectX::Decompress(cImages,  nimages, metadata, format, images);
}

HRESULT dxtGenerateMipMaps( const DirectX::Image& baseImage, int filter, int levels, DirectX::ScratchImage& mipChain, bool allow1D = false)
{
    std::cout << "DX A12\n";
	return DirectX::GenerateMipMaps(baseImage, (DirectX::TEX_FILTER_FLAGS) filter, levels, mipChain, allow1D);
}

HRESULT dxtGenerateMipMapsArray( const DirectX::Image* srcImages, int nimages, const DirectX::TexMetadata& metadata, int filter, int levels, DirectX::ScratchImage& mipChain )
{
    std::cout << "DX A11\n";
	return DirectX::GenerateMipMaps(srcImages, nimages, metadata, (DirectX::TEX_FILTER_FLAGS) filter, levels, mipChain);
}

HRESULT dxtGenerateMipMaps3D( const DirectX::Image* baseImages, int depth, int filter, int levels, DirectX::ScratchImage& mipChain )
{
    std::cout << "DX A10\n";
	return DirectX::GenerateMipMaps3D(baseImages, depth, (DirectX::TEX_FILTER_FLAGS) filter, levels, mipChain);
}

HRESULT dxtGenerateMipMaps3DArray( const DirectX::Image* srcImages, int nimages, const DirectX::TexMetadata& metadata, int filter, int levels, DirectX::ScratchImage& mipChain )
{
    std::cout << "DX A9\n";
	return DirectX::GenerateMipMaps3D(srcImages, nimages, metadata, (DirectX::TEX_FILTER_FLAGS) filter, levels, mipChain);
}

HRESULT dxtResize(const DirectX::Image* srcImages, int nimages, const DirectX::TexMetadata& metadata, int width, int height, int filter, DirectX::ScratchImage& result )
{
    std::cout << "DX A8\n";
	return DirectX::Resize(srcImages, nimages, metadata, width, height, (DirectX::TEX_FILTER_FLAGS) filter, result);
}

HRESULT dxtComputeNormalMap( const DirectX::Image* srcImages, int nimages, const DirectX::TexMetadata& metadata, int flags, float amplitude, DXGI_FORMAT format, DirectX::ScratchImage& normalMaps )
{
    std::cout << "DX A7\n";
	return DirectX::ComputeNormalMap(srcImages, nimages, metadata, (DirectX::CNMAP_FLAGS) flags, amplitude, format, normalMaps);
}

HRESULT dxtPremultiplyAlpha( const DirectX::Image* srcImages, int nimages, const DirectX::TexMetadata& metadata, int flags, DirectX::ScratchImage& result )
{
    std::cout << "DX A6\n";
	return DirectX::PremultiplyAlpha(srcImages, nimages, metadata, (DirectX::TEX_PMALPHA_FLAGS) flags, result);
}


// I/O functions
HRESULT dxtLoadDDSFile(LPCWSTR szFile, int flags, DirectX::TexMetadata* metadata, DirectX::ScratchImage& image)
{
    std::cout << "DX A5\n";
    char *fname = (char *)malloc(wcslen(szFile)*2+1);
	unsigned int i=0;
	for(; i < wcslen(szFile) * 2; i++) // convert 16-bit to 8-bit
		fname[i] = (char)(szFile[i] & 0x00FF);
	// set terminating 0
	fname[i]=0;
    std::cout << "DX C3 " << fname << " " << wcslen(szFile) << "\n";

	HRESULT result = DirectX::LoadFromDDSFile(szFile, (DirectX::DDS_FLAGS) flags, metadata, image);
    free (fname);

    std::cout << "DX C5 " << std::hex << result << "\n";
    std::cout << "DX C6 " << metadata->width << " " << metadata->height << " " << metadata->depth << " " << metadata->arraySize << " " << metadata->mipLevels << " " << metadata->format << "\n";
//     exit (0);
    return result;
}

HRESULT dxtLoadTGAFile(LPCWSTR szFile, DirectX::TexMetadata* metadata, DirectX::ScratchImage& image)
{
    std::cout << "DX A4\n";

//     char *fname = (char *)malloc(wcslen(szFile)*2+1);
// 	unsigned int i=0;
// 	for(; i < wcslen(szFile) * 2; i++) // convert 16-bit to 8-bit
// 		fname[i] = (char)(szFile[i] & 0x00FF);
// 	// set terminating 0
// 	fname[i]=0;

	HRESULT result = DirectX::LoadFromTGAFile(szFile, metadata, image);
//     free (fname);
    return result;
}

HRESULT dxtLoadWICFile(LPCWSTR szFile, int flags, DirectX::TexMetadata* metadata, DirectX::ScratchImage& image)
{
    std::cout << "DX A3\n";

//     char *fname = (char *)malloc(wcslen(szFile)*2+1);
// 	unsigned int i=0;
// 	for(; i < wcslen(szFile) * 2; i++) // convert 16-bit to 8-bit
// 		fname[i] = (char)(szFile[i] & 0x00FF);
// 	// set terminating 0
// 	fname[i]=0;

   	HRESULT result = 0x80004005;
// 	HRESULT result = DirectX::LoadFromWICFile(szFile, (DirectX::WIC_FLAGS) flags, metadata, image);
//     free (fname);
    return result;
}

HRESULT dxtSaveToDDSFile( const DirectX::Image& image, int flags, LPCWSTR szFile )
{
    std::cout << "DX A2\n";
//     char *fname = (char *)malloc(wcslen(szFile)*2+1);
// 	unsigned int i=0;
// 	for(; i < wcslen(szFile) * 2; i++) // convert 16-bit to 8-bit
// 		fname[i] = (char)(szFile[i] & 0x00FF);
// 	// set terminating 0
// 	fname[i]=0;

	HRESULT result = DirectX::SaveToDDSFile(image, (DirectX::DDS_FLAGS) flags, szFile);
//     free (fname);
    return result;
}

HRESULT dxtSaveToDDSFileArray( const DirectX::Image* images, int nimages, const DirectX::TexMetadata& metadata, int flags, LPCWSTR szFile )
{
    std::cout << "DX A1\n";
	return DirectX::SaveToDDSFile(images, nimages, metadata, (DirectX::DDS_FLAGS) flags, szFile);
}

// Scratch Image
DirectX::ScratchImage * dxtCreateScratchImage()
{
    std::cout << "Creating scratch image\n";
	return new DirectX::ScratchImage();
}

void dxtDeleteScratchImage(DirectX::ScratchImage * img) { delete img; }

HRESULT dxtInitialize(DirectX::ScratchImage * img, const DirectX::TexMetadata& mdata ) { return img->Initialize(mdata); }

HRESULT dxtInitialize1D(DirectX::ScratchImage * img, DXGI_FORMAT fmt,  int length,  int arraySize,  int mipLevels ) { return img->Initialize1D(fmt, length, arraySize, mipLevels); }
HRESULT dxtInitialize2D(DirectX::ScratchImage * img, DXGI_FORMAT fmt,  int width,  int height,  int arraySize,  int mipLevels ) { return img->Initialize2D(fmt, width, height, arraySize, mipLevels); }
HRESULT dxtInitialize3D(DirectX::ScratchImage * img, DXGI_FORMAT fmt,  int width,  int height,  int depth,  int mipLevels ) { return img->Initialize3D(fmt, width, height, depth, mipLevels); }
HRESULT dxtInitializeCube(DirectX::ScratchImage * img, DXGI_FORMAT fmt,  int width,  int height,  int nCubes,  int mipLevels ) { return img->InitializeCube(fmt, width, height, nCubes, mipLevels); }

HRESULT dxtInitializeFromImage(DirectX::ScratchImage * img, const DirectX::Image& srcImage, bool allow1D) { return img->InitializeFromImage(srcImage, allow1D); }
HRESULT dxtInitializeArrayFromImages(DirectX::ScratchImage * img, const DirectX::Image* images, int nImages, bool allow1D ) { return img->InitializeArrayFromImages(images, nImages, allow1D); }
HRESULT dxtInitializeCubeFromImages(DirectX::ScratchImage * img, const DirectX::Image* images,  int nImages ) { return img->InitializeCubeFromImages(images, nImages); }
HRESULT dxtInitialize3DFromImages(DirectX::ScratchImage * img, const DirectX::Image* images,  int depth ) { return img->Initialize3DFromImages(images, depth); }


void dxtRelease(DirectX::ScratchImage * img) { img->Release(); }

bool dxtOverrideFormat(DirectX::ScratchImage * img, DXGI_FORMAT f ) { return img->OverrideFormat(f); }

const DirectX::TexMetadata& dxtGetMetadata(const DirectX::ScratchImage * img) { return img->GetMetadata(); }
const DirectX::Image* dxtGetImage(const DirectX::ScratchImage * img, int mip,  int item,  int slice)  { return img->GetImage(mip, item, slice); }

const DirectX::Image* dxtGetImages(const DirectX::ScratchImage * img) { return img->GetImages(); }
int dxtGetImageCount(const DirectX::ScratchImage * img) { return img->GetImageCount(); }

uint8_t* dxtGetPixels(const DirectX::ScratchImage * img) { return img->GetPixels(); }
int dxtGetPixelsSize(const DirectX::ScratchImage * img) { return img->GetPixelsSize(); }
