#!/usr/bin/env python3
# SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
#
# SPDX-License-Identifier: GPL-3.0-only

import argparse
import os
import shutil
import subprocess

glsl_ext = ['.vert', '.frag']
debug = True

def compile_shader(res_root: str, path: str, filename: str) -> None:
    spirv_path = os.path.join(res_root, 'spirv')
    if not os.path.isdir(spirv_path):
        os.mkdir(spirv_path)
    cmd: list[str] = ['glslc', '--target-env=vulkan1.4', path, '-o', os.path.join(spirv_path, filename) + '.spv']
    if debug:
        cmd.append('-g')
    else:
        cmd.append('-O')
    print(' '.join(cmd))
    subprocess.run(cmd)

def compile_dir(dir: str) -> None:
    for root, dirs, files in os.walk(dir):
        for file in files:
            for ext in glsl_ext:
                if file.endswith(ext):
                    compile_shader(dir, os.path.join(root, file), file)

def clean(dir: str) -> None:
    shutil.rmtree(dir + '/spirv', ignore_errors=True)


if __name__ == '__main__':
    parser = argparse.ArgumentParser(
            prog='resc',
            description='Compile Malachite Resources')
    parser.add_argument('-d', '--res-dir', default='./res', help="The resource Directory")
    parser.add_argument('-g', '--debug-mode', action='store_true', default='false',
                        help="Compile with any applicable debug info")
    parser.add_argument('--glslc', default='glslc', help="glslc program from shaderc")
    parser.add_argument('operation', default='build', help="what operation to do: build or clean")

    args = parser.parse_args();
    dirroot = args.res_dir;
    debug = args.debug_mode;
    opt = args.operation;
    clean_only = False
    if opt == 'clean':
        clean_only = True
    elif opt == 'build':
        pass
    else:
        print("Allowed options: clean, build. No option means build")
        exit(1)

    clean(dirroot)
    if clean_only:
        exit(0)
    compile_dir(dirroot)
