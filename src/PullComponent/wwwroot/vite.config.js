import { defineConfig } from 'vite'

// 文件入口，input file
const entryFileNames = {
    PullRefresh: './src/PullRefresh.ts',
}

export default defineConfig(({ command, mode }) => {
    var isProd = mode === 'production';
    const isWatch = !isProd;
    console.log(process.env.buildWatch, isWatch)
    return {
        base: "./",
        // mode: mode, // 'development' 用于开发，'production' 用于构建
        build: {
            outDir: './dist',
            // assetsDir: 'assets',
            sourcemap: !isProd,
            chunkSizeWarningLimit: 20, // 20 KB
            watch: isWatch,
            lib: {
                fileName: "[name]",
                formats: ["es"],
            },
            rollupOptions: {
                input: entryFileNames,
                // 用于排除不需要打包的依赖
                // external: ["react", "react-dom"],
                output: {
                    //  chunk 包的文件名，默认 [name]-[hash].js
                    chunkFileNames: '[name].js',
                    // 定义 chunk 包的名 和规则
                    manualChunks: (id, { getModuleInfo }) => {
                        console.log("manualChunks", id, getModuleInfo(id))

                        // 打包其他依赖
                        if (id.includes('node_modules')) {
                            // 每个依赖打包成一个文件
                            // return "vendor/" + id.toString().split('node_modules/')[1].split('/')[0].toString()
                            return 'vendor/vendor';
                        }

                        // 打包自己代码中 import 多次的模块
                        const reg = /(.*)\/src\/(.*)/
                        if (reg.test(id)) {
                            const importersLen = getModuleInfo(id).importers.length;
                            console.log(id, importersLen)
                            // 被多处引用
                            if (importersLen > 1) {
                                return 'assets/common';
                            }
                        }
                    },
                    // 资源文件打包变量名， 默认值："assets/[name]-[hash][extname]"
                    assetFileNames: (fileInfo) => {
                        console.log("assetFileNames", fileInfo);
                        const fileName = fileInfo.name;
                        // js 是entry，所以这里不会生效
                        if (fileName.endsWith('.js') || fileName.endsWith('.ts')) {
                            return `[name].[ext]`
                        }

                        if (fileName.endsWith('.css')) {
                            return `[name].[ext]`
                        }

                        if (fileName.endsWith('.svg')
                            || fileName.endsWith('.png')
                            || fileName.endsWith('.jpg')
                            || fileName.endsWith('.jpeg')
                        ) {
                            return `image/[name].[ext]`
                        }

                        if (fileName.endsWith('.ttf')
                            || fileName.endsWith('.otf')
                            || fileName.endsWith('.eot')
                            || fileName.endsWith('.woff')
                            || fileName.endsWith('.woff2')
                        ) {
                            return `font/[name].[ext]`
                        }
                        return `[ext]/[name].[ext]`
                    },
                    // 入口文件 input 配置所指向的文件包名 默认值："[name].js" 
                    entryFileNames: (fileInfo) => {
                        console.log("entryFileNames", fileInfo.facadeModuleId);
                        return '[name].js';
                    },
                },
            },
        },
    }
}
)