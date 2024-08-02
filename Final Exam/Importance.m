function importanceMap = Importance(grayscaleImage)
    % Get the size of the grayscale image
    [height, width] = size(grayscaleImage);
    
    % Initialize the importance map with zeros
    importanceMap = zeros(height, width);
    
    % Loop over each pixel (excluding border pixels)
    for i = 2:height-1
        for j = 2:width-1
            % Get the current pixel and its neighbors
            currentPixel = grayscaleImage(i, j);
            leftPixel = grayscaleImage(i, j-1);
            rightPixel = grayscaleImage(i, j+1);
            upperPixel = grayscaleImage(i-1, j);
            lowerPixel = grayscaleImage(i+1, j);
            
            % Compute the importance value for the current pixel
            importanceValue = abs(currentPixel - leftPixel) + ...
                              abs(currentPixel - rightPixel) + ...
                              abs(currentPixel - upperPixel) + ...
                              abs(currentPixel - lowerPixel);
            
            % Store the importance value in the map
            importanceMap(i, j) = importanceValue;
        end
    end
    
    % Border pixels are left as zero (can change if needed)
end
